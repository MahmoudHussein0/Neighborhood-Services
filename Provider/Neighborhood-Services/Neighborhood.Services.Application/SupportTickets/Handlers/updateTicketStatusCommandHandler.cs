using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.SupportTickets.Commands;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    public class updateTicketStatusCommandHandler : IRequestHandler<updateTicketStatusCommand, SupportTicketDto>
    {
        private readonly ISupportTicketRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public updateTicketStatusCommandHandler(ISupportTicketRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }


        public async Task<SupportTicketDto> Handle(updateTicketStatusCommand request, CancellationToken cancellationToken)
        {


            var ticket = await _repository.GetByIdAsync(request.Id);
            if (ticket is null)
            {
                throw new Exception($"SupportTicket with id {request.Id} not found.");
            }

            if (!IsValidTransition(ticket.Status, request.Status))
            {
                throw new Exception(
                    $"Invalid status transition from {ticket.Status} to {request.Status}");
            }
            ticket.Status = request.Status;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            return SupportMapper.MapTicketToDto(ticket);
        }


        private static bool IsValidTransition(
            SupportTicketStatus currentStatus,
            SupportTicketStatus newStatus)
        {
            if (currentStatus == newStatus)
                return true;

            return currentStatus switch
            {
                SupportTicketStatus.Open =>
                    newStatus == SupportTicketStatus.InProgress,

                SupportTicketStatus.InProgress =>
                    newStatus == SupportTicketStatus.WaitingOnCustomer ||
                    newStatus == SupportTicketStatus.Resolved,

                SupportTicketStatus.WaitingOnCustomer =>
                    newStatus == SupportTicketStatus.InProgress ||
                    newStatus == SupportTicketStatus.Resolved,

                SupportTicketStatus.Resolved =>
                    false,

                _ => false
            };
        }
    }
   
}
