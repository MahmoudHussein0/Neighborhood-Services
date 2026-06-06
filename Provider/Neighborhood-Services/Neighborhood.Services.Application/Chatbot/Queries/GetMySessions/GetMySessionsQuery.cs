using MediatR;
using Neighborhood.Services.Application.Chatbot.DTOs;
using Neighborhood.Services.Application.Chatbot.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.Application.Chatbot.Queries.GetMySessions
{
    // Lists the current user's chat sessions (newest first).
    public class GetMySessionsQuery : IRequest<List<ChatSessionDto>>
    {
    }

    public class GetMySessionsQueryHandler : IRequestHandler<GetMySessionsQuery, List<ChatSessionDto>>
    {
        private readonly IChatbotRepository _chatbotRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMySessionsQueryHandler(IChatbotRepository chatbotRepository, ICurrentUserService currentUserService)
        {
            _chatbotRepository = chatbotRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<ChatSessionDto>> Handle(GetMySessionsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var sessions = await _chatbotRepository.GetUserSessionsAsync(userId);

            return sessions.Select(s => new ChatSessionDto
            {
                Id = s.Id,
                Title = s.Title,
                CreatedAt = s.CreatedAt
            }).ToList();
        }
    }
}
