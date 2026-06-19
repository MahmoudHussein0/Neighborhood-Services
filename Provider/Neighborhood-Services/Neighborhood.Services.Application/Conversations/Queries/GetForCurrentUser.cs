using MediatR;
using Neighborhood.Services.Application.Conversations.DTOs;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Conversations.Queries
{
    public class GetMyConvsQDto : IRequest<List<ConversationSelectedDto>>
    {
       
    }
    public class GetMyConvsQHandler : IRequestHandler<GetMyConvsQDto, List<ConversationSelectedDto>>
    {
        private readonly IConversationRepository _convrepository;
        private readonly ICurrentUserService _current;

        public GetMyConvsQHandler(IConversationRepository convrepository,ICurrentUserService Current)
        {
            _convrepository = convrepository;
            _current=Current;
        }
        public async Task <List<ConversationSelectedDto>> Handle(GetMyConvsQDto request, CancellationToken cancellationToken)
        {
            //var item =await  _convrepository.GetByBookingId(request.id);
            var items = await _convrepository.GetWithLastMessageSenderbyUserId(_current.UserId);
          // var image = await _convrepository.GetAvatar("0b6c2e03-1110-4cb6-9ad0-98c79ee97d1c");
            if (items == null) return null!;
            return items.Select(item => new ConversationSelectedDto
            {
                id = item.Id,
                bookingId = item.BookingId,
                lastMessage = (_convrepository.GetLastMessage(item.Id)).Result.content,
               // lastMessage = item.lastMessage.content,
                bookingDescription = item.Booking.Description,
                coversationImage= _convrepository.GetAvatar(item.Id,_current.UserId).Result??null!,
                othersName=_convrepository.GetOther(item.Id, _current.UserId).Result ?? null!,
                //coversationImage= item.lastMessage?.Sender?.Photo??null!,
                messageSenderId =item.lastMessage.SenderId,
                messageSenderName=item.lastMessage.Sender.FullName,
                updatedAt=item.lastMessage.createdAt,
               

            })
               .ToList();


        }

       
    }
}
