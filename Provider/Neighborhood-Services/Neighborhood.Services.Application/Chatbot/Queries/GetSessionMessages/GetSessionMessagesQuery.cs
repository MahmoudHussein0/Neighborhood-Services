using MediatR;
using Neighborhood.Services.Application.Chatbot.DTOs;
using Neighborhood.Services.Application.Chatbot.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Chatbot;

namespace Neighborhood.Services.Application.Chatbot.Queries.GetSessionMessages
{
    // Returns a session with its full message history (must belong to the current user).
    public class GetSessionMessagesQuery : IRequest<ChatSessionDetailDto>
    {
        public int SessionId { get; set; }
    }

    public class GetSessionMessagesQueryHandler : IRequestHandler<GetSessionMessagesQuery, ChatSessionDetailDto>
    {
        private readonly IChatbotRepository _chatbotRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetSessionMessagesQueryHandler(IChatbotRepository chatbotRepository, ICurrentUserService currentUserService)
        {
            _chatbotRepository = chatbotRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ChatSessionDetailDto> Handle(GetSessionMessagesQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var session = await _chatbotRepository.GetSessionWithMessagesAsync(request.SessionId)
                ?? throw new NotFoundException(nameof(ChatbotSession), request.SessionId);

            if (session.UserId != userId)
                throw new ForbiddenException("You don't have access to this chat session.");

            return new ChatSessionDetailDto
            {
                Id = session.Id,
                Title = session.Title,
                CreatedAt = session.CreatedAt,
                Messages = session.Messages
                    // Tool messages are internal context (replayed to the model), not shown in chat.
                    .Where(m => m.Role != ChatbotRole.Tool)
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => new ChatMessageDto
                    {
                        Role = m.Role.ToString(),
                        Content = m.Content,
                        CreatedAt = m.CreatedAt
                    }).ToList()
            };
        }
    }
}
