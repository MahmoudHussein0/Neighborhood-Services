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
            // 1. Who is chatting (null = guest, not logged in)
            var userId = _currentUserService.UserId;

            // 2. Load/create a session ONLY for logged-in users
            ChatbotSession? session = null;
            if (request.SessionId.HasValue)
            {
                // Continuing a saved session — must be logged in and own it
                session = await _chatbotRepository.GetSessionWithMessagesAsync(request.SessionId.Value)
                    ?? throw new NotFoundException(nameof(ChatbotSession), request.SessionId.Value);

                if (session.UserId != userId)
                    throw new ForbiddenException("You don't have access to this chat session.");
            }
            else if (userId is not null)
            {
                // New session for a logged-in user
                session = new ChatbotSession
                {
                    UserId = userId,
                    Title = request.Message.Length > 50 ? request.Message[..50] : request.Message,
                    CreatedAt = DateTime.UtcNow
                };
                await _chatbotRepository.AddAsync(session);
            }
            // Guest (userId == null, no SessionId) → session stays null, nothing persisted
            // SYSTEM PROMPT 
            // 3. Pull relevant knowledge from Qdrant for the user's message
            // 3. Pull relevant knowledge from Qdrant for the user's message
            var knowledge = await _memory.SearchAsync("platform-knowledge", request.Message, topK: 3);
            var context = string.Join("\n", knowledge);

            // 4. Build the system prompt with the retrieved context
            var systemPrompt = $"""
                  You are the booking assistant for "Neighborhood Services", a home services marketplace in Egypt.
                  Your job is to help customers understand the services, prices, and HOW to book.

                  Guidelines:
                  - Answer using the context below when relevant. If it doesn't contain the answer, say you're not sure and suggest contacting support.
                  - When a user wants to book, GUIDE them step by step (choose a service, pick a technician, choose a time, confirm) and tell them to use the booking page to complete it. Do NOT claim you booked anything yourself.
                  - If a user who is not logged in wants to book or do account actions, tell them they need to log in first.
                  - When asked about prices, give the range/estimate from the context.
                  - If the user writes in Arabic, reply in Arabic. If in English, reply in English. Keep replies concise and friendly.

                  Context:
                  {context}
                  """;

            // 5. Build ChatHistory from the session's past messages (only logged-in users have a session)
            var history = new ChatHistory();
            if (session is not null)
            {
                foreach (var msg in session.Messages.OrderBy(m => m.CreatedAt))
                {
                    if (msg.Role == ChatbotRole.User)
                        history.AddUserMessage(msg.Content);
                    else
                        history.AddAssistantMessage(msg.Content);
                }
            }
            // add the new user message
            history.AddUserMessage(request.Message);
            // 6. Call the AI
            var reply = await _aiClient.ChatAsync(history, systemPrompt);

            // 7. Save both messages — only if this is a logged-in user's session
            if (session is not null)
            {
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
            }

            // 8. Return the reply + session id (0 = guest, nothing saved)
            return new ChatReplyDto
            {
                SessionId = session?.Id ?? 0,
                Reply = reply
            };
        }


        }
}
