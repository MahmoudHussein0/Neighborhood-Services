using MediatR;
using Neighborhood.Services.Application.Messages.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Messages.Queries
{
    public class GetAllMssgsOfBookingQDto : IRequest<List<MessageSelectedDto>>
    {
        public int id { set; get; }
    }
    public class GetAllMssgsOfBookingQHandler : IRequestHandler<GetAllMssgsOfBookingQDto, List<MessageSelectedDto>>
    {
        private readonly IMessageRepository _messagerepository;

        public GetAllMssgsOfBookingQHandler(IMessageRepository messagerepository)
        {
            _messagerepository = messagerepository;
        }
        public async Task<List<MessageSelectedDto>> Handle(GetAllMssgsOfBookingQDto request, CancellationToken cancellationToken)
        {
            var items = await _messagerepository.GetByBookingIdAsync(request.id);
            if (items == null) return null!;
            return items.Select(item => new MessageSelectedDto
            {
                messageId = item.Id,
                conversationId = item.ConversationId,
                senderId = item.SenderId,
                senderName = item.Sender.FullName,
                content = item.content,
                read = item.isRead,
                deleted = item.IsDeleted,
                sentAt = item.createdAt,
                hasImage=item.hasImage,
                imageUrl=item.imageUrl,

            }).ToList();


        }


    }
}
