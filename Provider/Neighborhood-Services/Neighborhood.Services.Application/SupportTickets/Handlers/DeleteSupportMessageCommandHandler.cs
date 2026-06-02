using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.SupportTickets.Commands;
using Neighborhood.Services.Application.SupportTickets.Interfaces;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    public class DeleteSupportMessageCommandHandler : IRequestHandler<DeleteSupportMessageCommand, bool>
    {
        private readonly ISupportMessageRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSupportMessageCommandHandler(ISupportMessageRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSupportMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (message is null)
                throw new Exception($"SupportMessage with id {request.Id} not found.");

            message.IsDeleted = true;
            await _repository.UpdateAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
