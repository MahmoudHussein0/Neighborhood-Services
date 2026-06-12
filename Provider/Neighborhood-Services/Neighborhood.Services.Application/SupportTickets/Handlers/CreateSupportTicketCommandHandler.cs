using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.SupportTickets.Commands;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Domain.SupportTickets;

namespace Neighborhood.Services.Application.SupportTickets.Handlers
{
    public class CreateSupportTicketCommandHandler : IRequestHandler<CreateSupportTicketCommand, SupportTicketDto>
    {
        private readonly ISupportTicketRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IBookingRepository _bookingRepository;
        public CreateSupportTicketCommandHandler(ISupportTicketRepository repository, IUnitOfWork unitOfWork, ICurrentUserService currentUser, IBookingRepository bookingRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _bookingRepository = bookingRepository;
        }

        public async Task<SupportTicketDto> Handle(CreateSupportTicketCommand request, CancellationToken cancellationToken)
        {
            //if (request.BookingId.HasValue)
            //{
            //    var booking = await _bookingRepository.GetByIdAsync(
            //        request.BookingId.Value);

            //    if (booking is null)
            //    {
            //        throw new Exception(
            //            $"Booking with id {request.BookingId.Value} not found.");
            //    }
            //}
            var ticket = new SupportTicket
            {
                UserId = _currentUser.UserId,
                Subject = request.Subject,
                SenderName = request.SenderName,
                SenderEmail = request.SenderEmail,
                Description = request.Description,
                Status = SupportTicketStatus.Open,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _repository.AddAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            return SupportMapper.MapTicketToDto(ticket);
        }
    }

}
