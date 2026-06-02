using MediatR;
using Neighborhood.Services.Application.Conversations;
using Neighborhood.Services.Application.Conversations.DTOs;
using Neighborhood.Services.Application.Messages.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Messages.Queries
{
    public class GetAllMssgQHandler : IRequestHandler<IRequest<List<MessageSelectedDto>>, List<MessageSelectedDto>>
    {
        private readonly IMessageRepository _messagerepository;

        public GetAllMssgQHandler(IMessageRepository messagerepository)
        {
            _messagerepository = messagerepository;
        }
        public async Task<List<MessageSelectedDto>> Handle(IRequest<List<MessageSelectedDto>> request, CancellationToken cancellationToken)
        {
            var items = await _messagerepository.GetAllAsync();
            if (items.Count == 0) { return null; }
            return items.Select(item => new MessageSelectedDto
            {
                MessageId=item.Id,
                ConversationId=item.ConversationId,
                Content=item.content,
                Read=item.isRead,
                Deleted=item.IsDeleted
               
            })
                .ToList();
        }
    }
}
