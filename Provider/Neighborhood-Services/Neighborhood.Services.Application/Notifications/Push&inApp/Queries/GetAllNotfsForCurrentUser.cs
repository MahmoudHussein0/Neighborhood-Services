using MediatR;
using Neighborhood.Services.Application.Notifications.Push_inApp.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Exceptions;

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Neighborhood.Services.Application.Notifications.Push_inApp.Queries
{
    public class GetAllNotfsForCurrentUserQDto : IRequest<List<PushNotificationDto>>
    {
    }
    public class GetAllNotfsForCurrentUserQHnadler : IRequestHandler<GetAllNotfsForCurrentUserQDto, List<PushNotificationDto>>
    {
        private readonly INotificationsRepository _notfsrepository;
        private readonly ICurrentUserService _current;
        private readonly ILogger<GetAllNotfsForCurrentUserQHnadler> logger;


        public GetAllNotfsForCurrentUserQHnadler(INotificationsRepository notfrepository,
            ICurrentUserService current, ILogger<GetAllNotfsForCurrentUserQHnadler> Logger)
        {
            _notfsrepository = notfrepository;
            _current=current;
            logger= Logger;
        }
        public async Task<List<PushNotificationDto>> Handle(GetAllNotfsForCurrentUserQDto request, CancellationToken cancellationToken)
        {
            if (_current == null) { throw new UnauthorizedException("User is not authenticated.");}
            if (_current.UserId == null) { throw new UnauthorizedException("User ID is not authenticated."); }
            logger.LogInformation(_current.UserId);
            var item = await _notfsrepository.GetAllAsync(_current.UserId);
            if (item==null) { return null!; }
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
