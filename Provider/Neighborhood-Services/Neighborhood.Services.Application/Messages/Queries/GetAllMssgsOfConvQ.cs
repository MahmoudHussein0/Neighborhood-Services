using MediatR;
using Neighborhood.Services.Application.Messages.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Messages.Queries
{

  
    public class GetAllMssgsOfConvQDto : IRequest<List<MessageSelectedDto>>
    {
        public int id { set; get; }
    }
    public class GetAllMssgsOfConvQHandler : IRequestHandler<GetAllMssgsOfConvQDto, List<MessageSelectedDto>>
    {
        private readonly IMessageRepository _messagerepository;

        public GetAllMssgsOfConvQHandler(IMessageRepository messagerepository)
        {
            _messagerepository = messagerepository;
        }
        public async Task <List<MessageSelectedDto>> Handle(GetAllMssgsOfConvQDto request, CancellationToken cancellationToken)
        {
            var items = await _messagerepository.GetByConversationIdAsync(request.id);
            if (items == null) return null!;
            return items.Select(item => new MessageSelectedDto { 
            messageId=item.Id,
            conversationId=item.ConversationId,
                senderId = item.SenderId,
                senderName = item.Sender.FullName,
            content =item.content,
            read=item.isRead,
            deleted=item.IsDeleted,
            
            }).ToList();
              

        }

       
    }
}
