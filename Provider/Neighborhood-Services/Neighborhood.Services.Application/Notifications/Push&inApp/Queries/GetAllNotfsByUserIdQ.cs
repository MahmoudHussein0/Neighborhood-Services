using MediatR;
using Neighborhood.Services.Application.Notifications.Push_inApp.DTOs;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Notifications.Push_inApp.Queries
{
    public class GetAllNotfsByUserIdQDto : IRequest<List<PushNotificationDto>>
    {
       public string Id { set; get; }
    }
    public class GetAllNotfsByUserIdQHnadler : IRequestHandler<GetAllNotfsByUserIdQDto, List<PushNotificationDto>>
    {
        private readonly INotificationsRepository _notfsrepository;
        private readonly ICurrentUserService _current;


        public GetAllNotfsByUserIdQHnadler(INotificationsRepository notfrepository,
            ICurrentUserService current)
        {
            _notfsrepository = notfrepository;
            _current = current;
        }
        public async Task<List<PushNotificationDto>> Handle(GetAllNotfsByUserIdQDto request, CancellationToken cancellationToken)
        {
            //if (_current == null) { throw new UnauthorizedException("User is not authenticated.");}
            //if (_current.UserId == null) { throw new UnauthorizedException("User is not authenticated."); }

            var item = await _notfsrepository.GetAllAsync(request.Id);
            if (item==null) { return null; }
            return item.Select(notf => new PushNotificationDto
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
