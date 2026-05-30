using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.Notifications
{
    public class Notification : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
       public NotificationTypes type { get; set; }
      public string message { get; set; }
      public int refrenceId { get; set; }
     public bool isRead { get; set; }
     public NotificationChannels channel { get; set; }
        public DateTime createdAt { get; set; }

        //Nav Prop: User
        public ApplicationUser User { set; get; } = null;




    }
}
