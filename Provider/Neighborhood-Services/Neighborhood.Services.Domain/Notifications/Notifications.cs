using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.Notifications
{
    public class Notification: BaseEntity<int>
    {
      public int UserId {  get; set; }
       public NotificationTypes type { get; set; }
      public string message { get; set; }
      public int refrenceId { get; set; }
     public bool isRead { get; set; }
     public NotificationChannels channel { get; set; }
        public DateTime createdAt { get; set; }

        //Nav Prop: User
        public ApplicationUser.ApplicationUser User { set; get; } = new ApplicationUser.ApplicationUser();




    }
}
