using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Conversations.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Conversations.Commands
{

    public class DeleteConversationCommandDTO : IRequest<ConversationDeletedDto>
    {
        public int BookingId;
    }
    public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommandDTO, ConversationDeletedDto>
    {

        private readonly IConversationRepository _convrepository;
        private readonly IBookingRepository _bookrepository;

        private readonly IUnitOfWork _unitOfWork;

        public DeleteConversationCommandHandler(IConversationRepository Convrepository, IBookingRepository BookingRepository, IUnitOfWork unitOfWork)
        {
            _convrepository = Convrepository;
            _unitOfWork = unitOfWork;
            _bookrepository = BookingRepository;
        }//end of const.

        public async Task<ConversationDeletedDto> Handle(DeleteConversationCommandDTO request, CancellationToken cancellationToken)
        {
            var deletedconv = await _convrepository.GetByBookingId(request.BookingId);
            // var bb =_bookrepository.GetBookingWithDetailsAsync(request.BookingId);
            if (deletedconv == null) return null;



            await _convrepository.DeleteAsync(deletedconv.Id);
            await _unitOfWork.SaveChangesAsync();

            return new ConversationDeletedDto
            {
                BookingId = deletedconv.BookingId,
                ClientId = deletedconv.Messages.Last().SenderId,
                CreatedAt = deletedconv.createdAt,
                DeletedAt = DateTime.UtcNow
                

            };

        }//end of handling task 
    }
}
