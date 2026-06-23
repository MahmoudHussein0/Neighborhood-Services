using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
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
    public class CreateMessageCommandHandler: IRequestHandler<CreateMessageCommand, MessageCreatedDto>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingRepository _bookRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IChatService chatService;


        public CreateMessageCommandHandler(IMessageRepository messageRepository, 
            IConversationRepository conversationRepository,
            IUnitOfWork unitOfWork,
            IBookingRepository BookRepo,
            ICurrentUserService currentUserService,
            IChatService chatService)

        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _unitOfWork = unitOfWork;
            _bookRepo = BookRepo;
            _currentUserService = currentUserService;
            this.chatService = chatService;
            //user id to verify authorization
        }

        public async Task<MessageCreatedDto> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            var msg = new Message
            {
                SenderId = request.SenderId ?? _currentUserService.UserId ?? throw new Exception("Not Authenticated"),
                content = request.content,
                isRead = false,
                createdAt = DateTime.UtcNow,
                IsDeleted = false,
                hasImage=request.hasImage??false,
                imageUrl=request.imageUrl
            };

            //Verfiy There is a booking with this iD
            var selbook = await _bookRepo.GetByIdAsync(request.BookingId);
                if (selbook == null) { throw new Exception("No Booking With this Id"); };

            //adding message to conversation
            Conversation conv = await _conversationRepository.GetByBookingId(request.BookingId);

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
                    Booking = selbook,
                    Messages = new List<Message>(),
                 
                    //lastMessage=msg


                };
                conv.Messages.Add(msg);
                await _conversationRepository.AddAsync(conv);
                
                
            }
            //قبل ما نعمل سيند لمسدج معينة هيكون البوكنج اتكريت والناس انضافت للجروب
          

            await _messageRepository.AddAsync(msg);
            
            await _unitOfWork.SaveChangesAsync();

            await chatService.SendGroupMessage(request.BookingId.ToString(), new MessageCreatedDto()
            {
                id = msg.Id,

                content = msg.content,

                hasImage = msg.hasImage,
                imageUrl=msg.imageUrl
            });
            

            return new MessageCreatedDto() { 
                id=msg.Id,
                senderId = msg.SenderId,
                content=msg.content,
                BookingId=conv.BookingId,
                hasImage=msg.hasImage,
                imageUrl=msg.imageUrl
            };
            //return msg.Id;
        }
    }
}
