using MediatR;
using Microsoft.SemanticKernel.ChatCompletion;
using Neighborhood.Services.Application.AI.Interfaces;
using Neighborhood.Services.Application.Chatbot.DTOs;
using Neighborhood.Services.Application.Chatbot.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Chatbot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Chatbot.Commands.SendChatMessage
{
    public class SendChatMessageCommandHandler:IRequestHandler<SendChatMessageCommand, ChatReplyDto>
    {
        private readonly IChatbotRepository _chatbotRepository;
        private readonly IVectorMemory _memory;
        private readonly IAiClient _aiClient;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        public SendChatMessageCommandHandler(IChatbotRepository chatbotRepository,
                                                IVectorMemory vectorMemory,
                                                IAiClient aiClient,
                                                IUnitOfWork unitOfWork,
                                                ICurrentUserService currentUserService)
        {
            _chatbotRepository= chatbotRepository;
            _memory= vectorMemory;
            _aiClient= aiClient;
            _unitOfWork= unitOfWork;
            _currentUserService= currentUserService;
        }

        public async Task<ChatReplyDto> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
        {
            // 1. Who is chatting
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");
            ChatbotSession session;
            if (request.SessionId.HasValue)
            {
                session = await _chatbotRepository.GetSessionWithMessagesAsync(request.SessionId.Value)
                    ?? throw new NotFoundException(nameof(ChatbotSession), request.SessionId.Value);

                // Make sure this session belongs to the current user
                if (session.UserId != userId)
                    throw new ForbiddenException("You don't have access to this chat session.");
            }
            else
            {
                session = new ChatbotSession
                {
                    UserId = userId,
                    Title = request.Message.Length > 50 ? request.Message[..50] : request.Message,
                    CreatedAt = DateTime.UtcNow
                };
                await _chatbotRepository.AddAsync(session);
            }
            // SYSTEM PROMPT 
            // 3. Pull relevant knowledge from Qdrant for the user's message
            // 3. Pull relevant knowledge from Qdrant for the user's message
            var knowledge = await _memory.SearchAsync("platform-knowledge", request.Message, topK: 3);
            var context = string.Join("\n", knowledge);

            // 4. Build the system prompt with the retrieved context
            var systemPrompt = $"""
                  You are a helpful assistant for "Neighborhood Services", a home services marketplace in Egypt.
                  Answer the user's questions using the context below when relevant.
                  If the context doesn't contain the answer, say you're not sure and suggest contacting support.
                  If the user writes in Arabic, reply in Arabic. If in English, reply in English.

                  Context:
                  {context}
                  """;

            // 5. Build ChatHistory from the session's past messages
            var history = new ChatHistory();
            foreach (var msg in session.Messages.OrderBy(m => m.CreatedAt))
            {
                if (msg.Role == ChatbotRole.User)
                    history.AddUserMessage(msg.Content);
                else
                    history.AddAssistantMessage(msg.Content);
            }
            // add the new user message
            history.AddUserMessage(request.Message);
            // 6. Call the AI
            var reply = await _aiClient.ChatAsync(history, systemPrompt);

            // 7. Save the user message and the assistant reply
            session.Messages.Add(new ChatbotMessage
            {
                Role = ChatbotRole.User,
                Content = request.Message,
                CreatedAt = DateTime.UtcNow
            });
            session.Messages.Add(new ChatbotMessage
            {
                Role = ChatbotRole.Assistant,
                Content = reply,
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 8. Return the reply + session id
            return new ChatReplyDto
            {
                SessionId = session.Id,
                Reply = reply
            };
        }


        }
}
