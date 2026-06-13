using MediatR;
using Neighborhood.Services.Application.Conversations.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Conversations.Queries
{
    public class GetAllConvQDto: IRequest<List<ConversationSelectedDto>>
    {
    }

    public class GetAllConvQHandler : IRequestHandler<GetAllConvQDto, List<ConversationSelectedDto>>
    {
        private readonly IConversationRepository _convrepository;

        public GetAllConvQHandler(IConversationRepository convrepository)
        {
            _convrepository = convrepository;
        }
        public async Task <List<ConversationSelectedDto>> Handle(GetAllConvQDto request, CancellationToken cancellationToken)
        {
            var items = await _convrepository.GetAllAsync();
           
            return items.Select(item => new ConversationSelectedDto
            {
                id = item.Id,
                bookingId=item.BookingId,
                lastMessage= (_convrepository.GetLastMessage(item.Id)).Result.content
                // LastMessage = item.Messages.Count > 0 ? item.lastMessage.content : "no mssgs yet"

            })
                .ToList();
        }
    }


}

//          public int Id { get; set; }
//public int BookingId { get; set; }
//public string scssmsg { get; } = "Conversation Selected";
//public string LastMessage { set; get; }

