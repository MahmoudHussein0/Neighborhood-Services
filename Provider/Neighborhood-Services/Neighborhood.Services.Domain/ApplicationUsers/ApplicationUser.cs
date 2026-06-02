using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using System.Text;
using Neighborhood.Services.Domain.Payments;
using Neighborhood.Services.Domain.SupportTickets;
using Neighborhood.Services.Domain.PromoCodes;
using Neighborhood.Services.Domain.Notifications;
using Neighborhood.Services.Domain.Customers;
using Neighborhood.Services.Domain.Staffs;
using Neighborhood.Services.Domain.Technicians;
using Neighborhood.Services.Domain.Wallets;




namespace Neighborhood.Services.Domain.ApplicationUsers
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUserRole ApplicationUserRole { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Photo { get; set; } = string.Empty;
        public Point Location { get; set; } = null!;
        public string RefferalCode { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<PaymentMethod> paymentMethods { get; set; } =
            new List<PaymentMethod>();

        public ICollection<SupportTicket> SupportTickets { get; set; } =
            new List<SupportTicket>();

        public ICollection<PromoCodeUsage> PromoCodeUsages { get; set; } =
            new List<PromoCodeUsage>();

        public ICollection<SupportMessage> SupportMessages { get; set; } =
            new List<SupportMessage>();

        public ICollection<Notification> Notifications { get; set; } =
            new List<Notification>();

        public Customer Customer { get; set; } = null!;
        public Staff? Staff { get; set; } = null!;
        public Technician Technician { get; set; } = null!;
        public Wallet? Wallet { get; set; } = null!;





        
    
        //Nav Props added by Arwa
        public ICollection<Message.Message> Messages { get; set; } = new List<Message.Message>();
        //End of Arwa Additions



    }
}