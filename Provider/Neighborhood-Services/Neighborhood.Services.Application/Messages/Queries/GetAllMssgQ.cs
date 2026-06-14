using MediatR;
using Neighborhood.Services.Application.Conversations;
using Neighborhood.Services.Application.Conversations.DTOs;
using Neighborhood.Services.Application.Messages.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Messages.Queries
{
    public class GetAllMssgsQDto : IRequest<List<MessageSelectedDto>>
    {
       
    }
    public class GetAllMssgQHandler : IRequestHandler<GetAllMssgsQDto, List<MessageSelectedDto>>
    {
        private readonly IMessageRepository _messagerepository;

        public GetAllMssgQHandler(IMessageRepository messagerepository)
        {
            _messagerepository = messagerepository;
        }
        public async Task<List<MessageSelectedDto>> Handle(GetAllMssgsQDto request, CancellationToken cancellationToken)
        {
            var items = await _messagerepository.GetAllAsync();
            if (items.Count == 0) { return null; }
            return items.Select(item => new MessageSelectedDto
            {
                messageId=item.Id,
                conversationId=item.ConversationId,
                content=item.content,
                read=item.isRead,
                deleted=item.IsDeleted
               
            })
                .ToList();
        }
    }
}
