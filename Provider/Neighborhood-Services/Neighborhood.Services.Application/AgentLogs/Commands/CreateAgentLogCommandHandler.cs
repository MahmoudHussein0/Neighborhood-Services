using MediatR;
using Neighborhood.Services.Application.AgentLogs.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.AgentLogs;

namespace Neighborhood.Services.Application.AgentLogs.Commands
{
    public class CreateAgentLogCommandHandler : IRequestHandler<CreateAgentLogCommand>
    {
        private readonly IAgentLogRepository _agentLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAgentLogCommandHandler(IAgentLogRepository agentLogRepository, IUnitOfWork unitOfWork)
        {
            _agentLogRepository = agentLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateAgentLogCommand request, CancellationToken cancellationToken)
        {
            var log = new AgentLog
            {
                AgentType = request.AgentType,
                Action = request.Action,
                Input = request.Input,
                Output = request.Output,
                ReferenceType = request.ReferenceType,
                ReferenceId = request.ReferenceId,
                CreatedAt = DateTime.UtcNow,
                 TokensUsed = request.TokensUsed
            };

            await _agentLogRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
