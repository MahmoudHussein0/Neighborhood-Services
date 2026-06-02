using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.SupportTickets.Commands;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Application.Shared.Mappers;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    public class UpdateSupportTicketCommandHandler : IRequestHandler<UpdateSupportTicketCommand, SupportTicketDto>
    {
        private readonly ISupportTicketRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSupportTicketCommandHandler(ISupportTicketRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SupportTicketDto> Handle(UpdateSupportTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (ticket is null)
                throw new Exception($"SupportTicket with id {request.Id} not found.");

            ticket.Status = request.Status;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            return SupportMapper.MapTicketToDto(ticket);
        }
    }

}
