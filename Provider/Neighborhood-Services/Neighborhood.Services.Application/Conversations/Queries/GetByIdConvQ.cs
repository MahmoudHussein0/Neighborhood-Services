using MediatR;
using Neighborhood.Services.Application.Conversations.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Conversations.Queries
{

    public class GetByIdConvQDto : IRequest<ConversationSelectedDto>
    {
        public int id { set; get; }
    }
    public class GetByIdConvQHandler : IRequestHandler<GetByIdConvQDto, ConversationSelectedDto>
    {
        private readonly IConversationRepository _convrepository;

        public GetByIdConvQHandler(IConversationRepository convrepository)
        {
            _convrepository = convrepository;
        }
        public async Task<ConversationSelectedDto> Handle(GetByIdConvQDto request, CancellationToken cancellationToken)
        {
            var item =await  _convrepository.GetByBookingId(request.id);
            if (item == null) return null;
            return new ConversationSelectedDto()
            {
                Id = item.Id,
                BookingId = item.BookingId,
                LastMessage = item.lastMessage.content
            };
        
        }
    }
}
