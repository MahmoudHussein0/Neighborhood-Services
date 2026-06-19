using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.SupportTickets.Commands;
using Neighborhood.Services.Application.SupportTickets.Interfaces;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    public class DeleteSupportTicketCommandHandler : IRequestHandler<DeleteSupportTicketCommand, bool>
    {
        private readonly ISupportTicketRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSupportTicketCommandHandler(ISupportTicketRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSupportTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _repository.GetByIdAsync(request.Id);
            if (ticket is null)
                throw new Exception($"SupportTicket with id {request.Id} not found.");

            ticket.IsDeleted = true;
            await _repository.UpdateAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
