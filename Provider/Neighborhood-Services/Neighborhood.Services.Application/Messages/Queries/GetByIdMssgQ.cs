using MediatR;
using Neighborhood.Services.Application.Conversations;
using Neighborhood.Services.Application.Conversations.DTOs;
using Neighborhood.Services.Application.Messages.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Messages.Queries
{
    public class GetByIdMssgQDto : IRequest<MessageSelectedDto>
    {
        public int id { set; get; }
    }
    public class GetByIdConvQHandler : IRequestHandler<GetByIdMssgQDto, MessageSelectedDto>
    {
        private readonly IMessageRepository _messagerepository;

        public GetByIdConvQHandler(IMessageRepository messagerepository)
        {
            _messagerepository = messagerepository;
        }
        public async Task<MessageSelectedDto> Handle(GetByIdMssgQDto request, CancellationToken cancellationToken)
        {
            var item = await _messagerepository.GetByIdAsync(request.id);
            if (item == null) return null;
            return new MessageSelectedDto()
            {
                MessageId = item.Id,
                ConversationId = item.ConversationId,
                Content = item.content,
                Read = item.isRead,
                Deleted = item.IsDeleted
            };

        }
    }
}
