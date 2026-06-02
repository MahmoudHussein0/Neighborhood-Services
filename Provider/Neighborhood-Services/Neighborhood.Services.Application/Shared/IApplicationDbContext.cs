using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Domain.AgentLogs;
using Neighborhood.Services.Domain.AiAnalyses;
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

using Neighborhood.Services.Domain.Transactions;
using Neighborhood.Services.Domain.Wallets;
using Neighborhood.Services.Domain.TechnicianCategories;
using Neighborhood.Services.Domain.TechniciansAvailability;
using Neighborhood.Services.Domain.TechniciansPricing;

namespace Neighborhood.Services.Application.Shared
{
    public interface IApplicationDbContext
    {
        DbSet<AgentLog> AgentLogs { get; }
        DbSet<AiAnalysis> AiAnalyses { get; }
        DbSet<AvailabilityException> AvailabilityExceptions { get; }
        DbSet<Booking> Bookings { get; }
        DbSet<BookingImage> BookingImages { get; }
        DbSet<CancellationPolicy> CancellationPolicies { get; }
        DbSet<Category> Categories { get; }
        DbSet<Conversation> Conversations { get; }
        DbSet<Customer> Customers { get; }
        DbSet<CustomerAddress> CustomerAddresses { get; }
        DbSet<Dispute> Disputes { get; }
        DbSet<Escrow> Escrows { get; }
        DbSet<HistoricalPrice> HistoricalPrices { get; }
        DbSet<Invoice> Invoices { get; }
        DbSet<Message> Messages { get; }
        DbSet<Domain.Newsletter.Newsletter> Newsletters { get; }
        DbSet<Notification> Notifications { get; }
        DbSet<Offer> Offers { get; }
        DbSet<PaymentMethod> PaymentMethods { get; }
        DbSet<ProblemType> ProblemTypes { get; }
        DbSet<PromoCode> PromoCodes { get; }
        DbSet<PromoCodeUsage> PromoCodeUsages { get; }
        DbSet<RecurringBooking> RecurringBookings { get; }
        DbSet<Review> Reviews { get; }
        DbSet<ReviewAnalysis> ReviewAnalyses { get; }
        DbSet<ServiceRequest> ServiceRequests { get; }
        DbSet<Staff> Staffs { get; }
        DbSet<StaffPermission> StaffPermissions { get; }
        DbSet<SupportMessage> SupportMessages { get; }
        DbSet<SupportTicket> SupportTickets { get; }
        DbSet<Technician> Technicians { get; }
        DbSet<TechnicianAvailability> TechnicianAvailabilities { get; }
        DbSet<TechnicianCategory> TechnicianCategories { get; }
        DbSet<TechnicianPhoto> TechnicianPhotos { get; }
        DbSet<TechnicianPricing> TechnicianPricings { get; }
        DbSet<Transaction> Transactions { get; }
        DbSet<Wallet> Wallets { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
