using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.SupportTickets.Commands;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    // 🎯 نصيحة: خليت الكلاس public علشان الـ Dependency Injection يلقطه جوه الـ Web API من غير مشاكل
    public class updateTicketPriorityCommandHandler : IRequestHandler<updateTicketPriorityCommand, SupportTicketDto>
    {
        private readonly ISupportTicketRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public updateTicketPriorityCommandHandler(ISupportTicketRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SupportTicketDto> Handle(updateTicketPriorityCommand request, CancellationToken cancellationToken)
        {
         
            var ticket = await _repository.GetByIdAsync(request.Id);
            if (ticket is null)
            {
                throw new Exception($"SupportTicket with id {request.Id} not found.");
            }

            
            ticket.Priority = request.Priority;

           
            ticket.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            return SupportMapper.MapTicketToDto(ticket);
        }
    }
}