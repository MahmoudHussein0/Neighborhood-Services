using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.AgentLogs;
using Neighborhood.Services.Domain.AiAnalyses;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.AvilabilitiesException;
using Neighborhood.Services.Domain.BookingImages;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.CancellationPolicies;
using Neighborhood.Services.Domain.Categories;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.CustomerAddresses;
using Neighborhood.Services.Domain.Customers;
using Neighborhood.Services.Domain.Disputes;
using Neighborhood.Services.Domain.Escrows;
using Neighborhood.Services.Domain.HistoricalPrices;
using Neighborhood.Services.Domain.Invoices;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Domain.Notifications;
using Neighborhood.Services.Domain.Newsletter;
using Neighborhood.Services.Domain.Offers;
using Neighborhood.Services.Domain.Payments;
using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Domain.PromoCodes;
using Neighborhood.Services.Domain.RecurringBookings;
using Neighborhood.Services.Domain.Reviews;
using Neighborhood.Services.Domain.ServiceRequests;
using Neighborhood.Services.Domain.Staffs;
using Neighborhood.Services.Domain.SupportTickets;
using Neighborhood.Services.Domain.TechnicianPhotos;
using Neighborhood.Services.Domain.Technicians;
using Neighborhood.Services.Domain.TechnicionCategories;
using Neighborhood.Services.Domain.TechnicionsAvailability;
using Neighborhood.Services.Domain.TechnicionsPricing;
using Neighborhood.Services.Domain.Transactions;
using Neighborhood.Services.Domain.Wallets;
using System.Reflection;

namespace Neighborhood.Services.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<AgentLog> AgentLogs { get; set; }
        public DbSet<AiAnalysis> AiAnalyses { get; set; }
        public DbSet<AvailabilityException> AvailabilityExceptions { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingImage> BookingImages { get; set; }
        public DbSet<CancellationPolicy> CancellationPolicies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
        public DbSet<Escrow> Escrows { get; set; }
        public DbSet<HistoricalPrice> HistoricalPrices { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<ProblemType> ProblemTypes { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<PromoCodeUsage> PromoCodeUsages { get; set; }
        public DbSet<RecurringBooking> RecurringBookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewAnalysis> ReviewAnalyses { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<StaffPermission> StaffPermissions { get; set; }
        public DbSet<SupportMessage> SupportMessages { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        public DbSet<TechnicianAvailability> TechnicianAvailabilities { get; set; }
        public DbSet<TechnicianCategory> TechnicianCategories { get; set; }
        public DbSet<TechnicianPhoto> TechnicianPhotos { get; set; }
        public DbSet<TechnicianPricing> TechnicianPricings { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
