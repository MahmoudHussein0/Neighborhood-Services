using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Neighborhood.Services.Application.AI.Interfaces;
using Neighborhood.Services.Application.Bookings.Services;
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

        private readonly IPriceEstimationService _priceEstimationService;
        private readonly ILogger<SendChatMessageCommandHandler> _logger;

        // Minimum cosine similarity for the problem-type classifier to be trusted.
        // Below this, the user's message isn't clearly about any specific service —
        // we skip the price lookup and let the chatbot fall back to general RAG answer.
        private const float ClassifierConfidenceThreshold = 0.5f;


        public SendChatMessageCommandHandler(IChatbotRepository chatbotRepository,
                                                IVectorMemory vectorMemory,
                                                IAiClient aiClient,
                                                IUnitOfWork unitOfWork,
                                                ICurrentUserService currentUserService,
                                                IPriceEstimationService priceEstimationService,
                                                ILogger<SendChatMessageCommandHandler> logger)
        {
            _chatbotRepository= chatbotRepository;
            _memory= vectorMemory;
            _aiClient= aiClient;
            _unitOfWork= unitOfWork;
            _currentUserService= currentUserService;
            _priceEstimationService = priceEstimationService;
            _logger = logger;
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

            // 3. RAG: pull general platform knowledge for the user's message.
            var knowledge = await _memory.SearchAsync("platform-knowledge", request.Message, topK: 3);
            var context = string.Join("\n", knowledge);

            // 4. Classify the message against the problem-types collection.
            //    If the top hit is confident enough, ask Alaa's price service for a
            //    grounded estimate (uses HistoricalPrices + region) and inject it
            //    into the prompt as authoritative pricing context.
            //    If low confidence or anything fails, we silently skip — the chatbot
            //    falls back to whatever pricing info RAG retrieved.
            string? priceContext = null;
            try
            {
                var hits = await _memory.SearchDetailedAsync("problem-types", request.Message, topK: 1);
                var top = hits.FirstOrDefault();

                if (top is null)
                {
                    _logger.LogInformation("Chatbot classifier: no hits for '{Message}'", request.Message);
                }
                else
                {
                    var hasId = top.Fields.TryGetValue("problemTypeId", out var idStr);
                    _logger.LogInformation(
                        "Chatbot classifier: top score={Score:F3} (threshold={Threshold}) hasId={HasId} idStr={IdStr}",
                        top.Score, ClassifierConfidenceThreshold, hasId, idStr);

                    if (top.Score >= ClassifierConfidenceThreshold
                        && hasId
                        && int.TryParse(idStr, out var problemTypeId))
                    {
                        var estimate = await _priceEstimationService.EstimateAsync(problemTypeId, request.Region);
                        var regionPart = string.IsNullOrWhiteSpace(request.Region)
                            ? "(general average)"
                            : $"(based on {request.Region})";
                        priceContext = $"Estimated price for this service: ~{estimate:0.##} EGP {regionPart}.";
                        _logger.LogInformation("Chatbot classifier: injected priceContext='{PriceContext}'", priceContext);
                    }
                }
            }
            catch (Exception ex)
            {
                // Price lookup is a nice-to-have — never fail the chat over it.
                _logger.LogWarning(ex, "Chatbot classifier/price lookup failed; falling back to RAG only.");
            }

            // 5. Build the system prompt. RAG context is unchanged. If we have a grounded
            //    estimate, we hoist it to the TOP as a strict pricing directive so the model
            //    doesn't default to the wider range that's also present inside Context.
            var pricingDirective = priceContext is null ? "" : $"""

                === AUTHORITATIVE PRICING (use this exact number when the user asks about price) ===
                {priceContext}
                When the user asks about price, you MUST use the number above and mention the region if given.
                Do NOT quote the wider min-max range from the Context section instead. Phrase it as approximate
                (e.g. "around X EGP, depending on your specifics"), never as a fixed quote.
                ===============================================================================
                """;

            var systemPrompt = $"""
                  You are the booking assistant for "Neighborhood Services", a home services marketplace in Egypt.
                  Your job is to help customers understand the services, prices, and HOW to book.
                  {pricingDirective}
                  Guidelines:
                  - Answer using the context below when relevant. If it doesn't contain the answer, say you're not sure and suggest contacting support.
                  - When a user wants to book, GUIDE them step by step (choose a service, pick a technician, choose a time, confirm) and tell them to use the booking page to complete it. Do NOT claim you booked anything yourself.
                  - If a user who is not logged in wants to book or do account actions, tell them they need to log in first.
                  - When asked about prices and no AUTHORITATIVE PRICING block is present above, use the price range from the context. Phrase estimates as approximate.
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
