using MediatR;
using Neighborhood.Services.Application.Conversations.DTOs;
using Neighborhood.Services.Application.Notifications.Push_inApp.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Notifications.Push_inApp.Queries
{
    public class GetAllNotfsQDto : IRequest<List<PushNotificationDto>>
    {
    }
    public class GetAllNotfsQHandler : IRequestHandler <GetAllNotfsQDto, List<PushNotificationDto>>
    {
        private readonly INotificationsRepository _notfsrepository;

        public GetAllNotfsQHandler(INotificationsRepository notfrepository)
        {
            _notfsrepository = notfrepository;
        }
        public async Task<List<PushNotificationDto>> Handle(GetAllNotfsQDto request, CancellationToken cancellationToken)
        {
            var items = await _notfsrepository.GetAllAsync();
            if (items.Count == 0) { return null; }
            return items.Select(notf => new PushNotificationDto
            {
                Id = notf.Id,
                UserId = notf.UserId,
                Message = notf.message,
                CreatedDate = notf.createdAt,
                IsRead = notf.isRead,

            })
                .ToList();
        }

      
    }
}
