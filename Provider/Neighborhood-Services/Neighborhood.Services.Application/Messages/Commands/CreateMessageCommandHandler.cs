using Neighborhood.Services.Application.Conversations;
using Neighborhood.Services.Application.Messages;
using Neighborhood.Services.Application.Messages.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Messages.Commands
{
    public class CreateMessageCommandHandler
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IUnitOfWork _unitOfWork;


        public CreateMessageCommandHandler(IMessageRepository messageRepository, IConversationRepository conversationRepository,IUnitOfWork unitOfWork)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _unitOfWork= unitOfWork;
            //user id to verify authorization
        }

        public async Task<MessageCreatedDto> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            var msg = new Message
            {
                ConversationId = request.ConversationId,
                SenderId = request.SenderId,
                content = request.content,
                isRead = request.isRead
            };

            //adding message to conversation
            Conversation conv = await _conversationRepository.GetByIdAsync(msg.ConversationId);

            //if it exists:
            if (conv != null)
            {
                conv.Messages.Add(msg);
                await _conversationRepository.UpdateAsync(conv);

            }

            //if it doesn't exist then create the conversation
            if (conv == null)
            {
               
                conv = new Conversation
                {
                    BookingId = request.BookingId,
                    createdAt = DateTime.UtcNow,
                    //Booking = await selbook,
                    Messages = new List<Message>()


                };
                conv.Messages.Add(msg);
                await _conversationRepository.AddAsync(conv);

            }
            await _messageRepository.AddAsync(msg);
            
            await _unitOfWork.SaveChangesAsync();
            //sending notifications

            return new MessageCreatedDto() { senderId = request.SenderId ,content=request.content,BookingId=conv.BookingId};
            //return msg.Id;
        }
    }
}
