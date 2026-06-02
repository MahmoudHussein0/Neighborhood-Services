using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Conversations.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Conversations.Commands
{

    //cinversation is updated by adding new messages! that's all. 
    //we can't update anything else! وإلا نعتبر مزورين 
    public class UpdateConversationCommandDTO : IRequest<ConversationUpdatedDto>
    {
        public int BookingId { get; set; }

        public string senderId { get; set; }
        public string Message {  get; set; }


    }//end of update dto class


    public class UpdateConversationHandler : IRequestHandler<UpdateConversationCommandDTO, ConversationUpdatedDto>
    {

        private readonly IConversationRepository _convrepository;
        //private readonly IBookingRepository _bookrepository;

        private readonly IUnitOfWork _unitOfWork;
        public UpdateConversationHandler(IConversationRepository convrepository, IUnitOfWork unitOfWork)
        {
            _convrepository = convrepository;
            _unitOfWork = unitOfWork;
        }//end of ctr

        public async Task<ConversationUpdatedDto> Handle(UpdateConversationCommandDTO request, CancellationToken cancellationToken)
        {
            var updatedconv = await _convrepository.GetByBookingId(request.BookingId);
            if (updatedconv == null) return null;
            var addedMsg = new Message() {
                content = request.Message ,
                SenderId=request.senderId,
                ConversationId=updatedconv.Id }; 
                updatedconv.Messages.Add(addedMsg);

            await _convrepository.UpdateAsync(updatedconv);
            await _unitOfWork.SaveChangesAsync();

            Console.WriteLine("Conversation Updated!");
            Console.WriteLine(updatedconv.lastMessage.content);

            return new ConversationUpdatedDto() { 
            BookingId = updatedconv.BookingId,
            Message=updatedconv.Messages.LastOrDefault().content,
            MessageSenderId=updatedconv.Messages.LastOrDefault().SenderId,
            MessageSenderName=updatedconv.Messages.LastOrDefault().Sender.UserName,
            LastMessage=updatedconv.lastMessage.content,
            UpdatedAt=DateTime.UtcNow
    

    };//end of returned DTO

            //Sending Notification
            

                
                }//end of handling

    }//End of update Handler
    }
