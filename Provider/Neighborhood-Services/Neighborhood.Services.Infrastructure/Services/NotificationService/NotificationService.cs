
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Notifications;
using Neighborhood.Services.Application.Notifications.Push_inApp.DTOs;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Domain.Notifications;
using System.Threading.Channels;

namespace Neighborhood.Services.Infrastructure.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        //  private readonly ILogger _logger;
        private readonly ILogger<NotificationService> _logger;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _current;

        public NotificationService(IHubContext<NotificationHub> hubContext, 
            ILogger<NotificationService> logger,
            INotificationsRepository NotificationsRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService current)
        {
            _hubContext = hubContext; ;
            _logger = logger;
            _notificationsRepository= NotificationsRepository;
            _unitOfWork = unitOfWork;
            _current = current;
            
           
        }
        public async Task<PushNotificationDto> SendNotificationAsync(string mssg)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new { mssg });
            var notf = new Notification()
            {
                channel = NotificationChannels.push,
                createdAt = DateTime.UtcNow,
                message = mssg,
                IsDeleted = false,
                isRead = false,
                UserId = _current.UserId?? "45300128-25f9-4360-9229-fa3bf823f58e",
                type = Domain.Notifications.NotificationTypes.general
            };
            await _notificationsRepository.AddAsync(notf);
            await _unitOfWork.SaveChangesAsync();
            return new PushNotificationDto()
            {
                Id = notf.Id,
                UserId=notf.UserId,
                Message=notf.message,
                CreatedDate=notf.createdAt,
                IsRead=notf.isRead,
            };


        }

        public async Task<PushNotificationDto> SendNotificationToAdmin(string mssg)
        {
            
            await _hubContext.Clients.Group("Staff").SendAsync("ReceiveNotification", new { mssg });
           _logger.LogInformation($"sending to Admins: {mssg}");
            var notf = new Notification()
            {
                channel = NotificationChannels.push,
                createdAt = DateTime.UtcNow,
                message = mssg,
                IsDeleted = false,
                isRead = false,
                UserId = _current.UserId ?? "1",
                type = Domain.Notifications.NotificationTypes.general,
                refrenceId = 1
            };
            //([IsDeleted], [UserId], [channel], [createdAt], [isRead], [message], [refrenceId], [type]
            await _notificationsRepository.AddAsync(notf);
            await _unitOfWork.SaveChangesAsync();
            
            return new PushNotificationDto()
            {
                Id = notf.Id,
                UserId = notf.UserId,
                Message = notf.message,
                CreatedDate = notf.createdAt,
                IsRead = notf.isRead,
            };



        }

        public async Task<PushNotificationDto> SendNotificationToAdminIdentity(string mssg)
        {
            await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", new { mssg });
            var notf = new Notification()
            {
                channel = NotificationChannels.push,
                createdAt = DateTime.UtcNow,
                message = mssg,
                IsDeleted = false,
                isRead = false,
                UserId = _current.UserId ?? "1",
                type = Domain.Notifications.NotificationTypes.general
            };
            await _notificationsRepository.AddAsync(notf);
            await _unitOfWork.SaveChangesAsync();

            return new PushNotificationDto()
            {
                Id = notf.Id,
                UserId = notf.UserId,
                Message = notf.message,
                CreatedDate = notf.createdAt,
                IsRead = notf.isRead,
            };
        }

        public async Task<PushNotificationDto> SendNotificationToCustomer(string mssg)
        {
            await _hubContext.Clients.Group("Customer").SendAsync("ReceiveNotification", new { mssg });
            var notf = new Notification()
            {
                channel = NotificationChannels.push,
                createdAt = DateTime.UtcNow,
                message = mssg,
                IsDeleted = false,
                isRead = false,
                UserId = _current.UserId ?? "1",
                type = Domain.Notifications.NotificationTypes.general
            };
            await _notificationsRepository.AddAsync(notf);
            await _unitOfWork.SaveChangesAsync();

            return new PushNotificationDto()
            {
                Id = notf.Id,
                UserId = notf.UserId,
                Message = notf.message,
                CreatedDate = notf.createdAt,
                IsRead = notf.isRead,
            };
        }

        public async Task<PushNotificationDto> SendNotificationToTechnician(string mssg)
        {
            await _hubContext.Clients.Group("Technician").SendAsync("ReceiveNotification", new { mssg });
            var notf = new Notification()
            {
                channel = NotificationChannels.push,
                createdAt = DateTime.UtcNow,
                message = mssg,
                IsDeleted = false,
                isRead = false,
                UserId = _current.UserId ?? "1",
                type = Domain.Notifications.NotificationTypes.general
            };
            await _notificationsRepository.AddAsync(notf);
            await _unitOfWork.SaveChangesAsync();

            return new PushNotificationDto()
            {
                Id = notf.Id,
                UserId = notf.UserId,
                Message = notf.message,
                CreatedDate = notf.createdAt,
                IsRead = notf.isRead,
            };
        }

        //Customer,
        //Technician,
        //Staff

        public async Task<PushNotificationDto> SendNotificationToUser(string userId, string mssg)
        {
            await _hubContext.Clients.Group($"business-{userId}").SendAsync("ReceiveNotification", new { mssg });
            var notf = new Notification()
            {
                channel = NotificationChannels.push,
                createdAt = DateTime.UtcNow,
                message = mssg,
                IsDeleted = false,
                isRead = false,
                UserId = userId,
                type = Domain.Notifications.NotificationTypes.general
            };
            await _notificationsRepository.AddAsync(notf);
            await _unitOfWork.SaveChangesAsync();

            return new PushNotificationDto()
            {
                Id = notf.Id,
                UserId = notf.UserId,
                Message = notf.message,
                CreatedDate = notf.createdAt,
                IsRead = notf.isRead,
            };

        }

        //Customer,
        //Technician,
        //Staff

        public async Task<PushNotificationDto> SendRoleBasedNotificationAsync(string mssg,ApplicationUserRole userRole ,string? recipientUserId = null)
        {

            if (recipientUserId != null) {
                await SendNotificationToUser(recipientUserId, mssg);  
            }

            else if (Enum.IsDefined(userRole))
            {
                switch (userRole)
                {
                    case (ApplicationUserRole.Staff):
                        await SendNotificationToAdmin(mssg);
                        break;

                    case (ApplicationUserRole.Customer):
                        // if (!string.IsNullOrEmpty(recipientUserId))
                        await SendNotificationToCustomer(mssg);
                        break;
                    case (ApplicationUserRole.Technician):
                        // if (!string.IsNullOrEmpty(recipientUserId))
                        await SendNotificationToTechnician(mssg);
                        break;

                    default:

                        break;
                }
            }
            var notf = new Notification()
            {
                channel = NotificationChannels.push,
                createdAt = DateTime.UtcNow,
                message = mssg,
                IsDeleted = false,
                isRead = false,
                UserId = _current.UserId??"1",
                type = Domain.Notifications.NotificationTypes.general
            };
          //  await _notificationsRepository.AddAsync(notf);
          //  await _unitOfWork.SaveChangesAsync();

            return new PushNotificationDto()
            {
                Id = notf.Id,
                UserId = notf.UserId,
                Message = notf.message,
                CreatedDate = notf.createdAt,
                IsRead = notf.isRead,
            };
        }
    }
}
