using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Notifications.Push_inApp.DTOs
{
    public class PushNotificationDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public string UserId { get; set; } // who will see this        
        public DateTime CreatedDate { get; set; }
    }
}
