using MediatR;
using Neighborhood.Services.Application.Conversations.DTOs;
using Neighborhood.Services.Application.Notifications.Push_inApp.DTOs;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Notifications.Push_inApp.Commands
{
    public class MarkAsReadCommandDto : IRequest<PushNotificationDto>
    {
        public int NotificationId { set; get; }
    }
    public class MarkAsReadCommand : IRequestHandler<MarkAsReadCommandDto,PushNotificationDto>
    {
        private readonly INotificationsRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public MarkAsReadCommand(INotificationsRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }
        

        public async Task<PushNotificationDto> Handle(MarkAsReadCommandDto request, CancellationToken cancellationToken)
        {

            await _repository.MarkAsReadAsync(request.NotificationId);
            var selected = await _repository.GetByIdAsync(request.NotificationId);
            if (selected == null) { return null; }
            


            return new PushNotificationDto() {
            UserId=_currentUser.UserId,
            Id=request.NotificationId,
            CreatedDate=DateTime.UtcNow,
            IsRead=selected.isRead,
            Message=selected.message,
            
            };

        }

       
    }
}
