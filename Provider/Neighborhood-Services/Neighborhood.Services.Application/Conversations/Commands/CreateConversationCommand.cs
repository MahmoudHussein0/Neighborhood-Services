using Azure.Core;
using MediatR;
using Microsoft.Identity.Client;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Conversations.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Message;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Neighborhood.Services.Application.Conversations.Commands
{
    public class CreateConversationCommandDTO : IRequest<ConversationCreatedDto>
    {
        public int BookingId {  get; set; }
        //public string ClientId { get; set; }
        //public string TechnicianId { get; set;}
        //public string TechnicianName { get; }

    }//end of dto class

    public class CreateConversationCommandDTOHandler : IRequestHandler<CreateConversationCommandDTO, ConversationCreatedDto>
    {

        private readonly IConversationRepository _convrepository;
        private readonly IBookingRepository _bookrepository;

        private readonly IUnitOfWork _unitOfWork;

        public CreateConversationCommandDTOHandler(IConversationRepository Convrepository,IBookingRepository BookingRepository, IUnitOfWork unitOfWork)
        {
            _convrepository = Convrepository;
            _unitOfWork = unitOfWork;
            _bookrepository = BookingRepository;
        }//end of const.

        public async Task<ConversationCreatedDto> Handle(CreateConversationCommandDTO request, CancellationToken cancellationToken)
        {
            //worst perfofrmance at high loads (blocking)
        //    var selbook= _bookrepository.GetByIdAsync(request.BookingId);
        //    if ( selbook.Result == null) { return null; }

            //better (non blocking)

            var selbook = await _bookrepository.GetByIdAsync(request.BookingId);
            if (selbook == null) { return null; }
            // var bb =_bookrepository.GetBookingWithDetailsAsync(request.BookingId);

            var Conversation = new Conversation
            {
                BookingId = request.BookingId,
                createdAt = DateTime.UtcNow,
                Booking = selbook,
                Messages = new List<Message>()


            };
            await _convrepository.AddAsync(Conversation);
            await _unitOfWork.SaveChangesAsync();

            return new ConversationCreatedDto
            {
                BookingId = request.BookingId,
                ClientId = selbook.CustomerId,
                TechnicianId = selbook.TechnicianId,
               
                CreatedAt=Conversation.createdAt

            };

         }//end of handling task
    }
}
