using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Application.AgentLogs.Interfaces;
using Neighborhood.Services.Application.AiAnalysises.Interface;
using Neighborhood.Services.Application.AvilabilitiesException;
using Neighborhood.Services.Application.BookingImages.Interface;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.CancellationPolicies.Interfaces;
using Neighborhood.Services.Application.Categories;
using Neighborhood.Services.Application.Conversations;
using Neighborhood.Services.Application.CustomerAddresses.Interfaces;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Disputes.Interfaces;
using Neighborhood.Services.Application.Escrows.Interfaces;
using Neighborhood.Services.Application.HistoricalPrices;
using Neighborhood.Services.Application.Invoices.Interfaces;
using Neighborhood.Services.Application.Messages;
using Neighborhood.Services.Application.Notifications;
using Neighborhood.Services.Application.Offers.Interfaces;
using Neighborhood.Services.Application.Payments.Interfaces;
using Neighborhood.Services.Application.ProblemTypes;
using Neighborhood.Services.Application.PromoCodes.Interface;
using Neighborhood.Services.Application.RecurringBookings.Interfaces;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Application.TechnicianPhotos.Interfaces;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Application.TechnitianAvailability;
using Neighborhood.Services.Application.TechnitianPricing;
using Neighborhood.Services.Application.Transactions.Interfaces;
using Neighborhood.Services.Application.Users.Interfaces;
using Neighborhood.Services.Application.Wallets.Interfaces;
using Neighborhood.Services.Infrastructure.Persistence.AgentLogs;
using Neighborhood.Services.Infrastructure.Persistence.AiAnalysises;
using Neighborhood.Services.Infrastructure.Persistence.AvilabilitiesException;
using Neighborhood.Services.Infrastructure.Persistence.BookingImages;
using Neighborhood.Services.Infrastructure.Persistence.Bookings;
using Neighborhood.Services.Infrastructure.Persistence.CancellationPolicies;
using Neighborhood.Services.Infrastructure.Persistence.Categories;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Persistence.Conversations;
using Neighborhood.Services.Infrastructure.Persistence.CustomerAddresses;
using Neighborhood.Services.Infrastructure.Persistence.Customers;
using Neighborhood.Services.Infrastructure.Persistence.Disputes.Repository;
using Neighborhood.Services.Infrastructure.Persistence.Escrows;
using Neighborhood.Services.Infrastructure.Persistence.HistoricalPrices;
using Neighborhood.Services.Infrastructure.Persistence.Invoices;
using Neighborhood.Services.Infrastructure.Persistence.Messages;
using Neighborhood.Services.Infrastructure.Persistence.Newsletters;
using Neighborhood.Services.Infrastructure.Persistence.Offers;
using Neighborhood.Services.Infrastructure.Persistence.Payments;
using Neighborhood.Services.Infrastructure.Persistence.ProblemTypes;
using Neighborhood.Services.Infrastructure.Persistence.PromoCodes;
using Neighborhood.Services.Infrastructure.Persistence.RecurringBookings;
using Neighborhood.Services.Infrastructure.Persistence.Reviews.Repository;
using Neighborhood.Services.Infrastructure.Persistence.ServiceRequests;
using Neighborhood.Services.Infrastructure.Persistence.Staffs.Repository;
using Neighborhood.Services.Infrastructure.Persistence.SupportTickets.Repository;
using Neighborhood.Services.Infrastructure.Persistence.TechnicianPhotos;
using Neighborhood.Services.Infrastructure.Persistence.Technicians;
using Neighborhood.Services.Infrastructure.Persistence.TechnitianAvailability;
using Neighborhood.Services.Infrastructure.Persistence.TechnitianPricing;
using Neighborhood.Services.Infrastructure.Persistence.Transactions;
using Neighborhood.Services.Infrastructure.Persistence.Users;
using Neighborhood.Services.Infrastructure.Persistence.Wallets;
using Neighborhood.Services.Infrastructure.Services;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure
{
    public static class DependencyInjection
    {
        public static  IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //  repositories
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingImageRepository, BookingImageRepository>();
            services.AddScoped<IAiAnalysisRepository, AiAnalysisRepository>();
            services.AddScoped<IAgentLogRepository, AgentLogRepository>();
            services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
            services.AddScoped<IOfferRepository, OfferRepository>();
            services.AddScoped<IRecurringBookingRepository, RecurringBookingRepository>();
            services.AddScoped<ICancellationPolicyRepository, CancellationPolicyRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            //--
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IEscrowRepository, EscrowRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            services.AddScoped<ITechnicianRepository, TechnicianRepository>();
            services.AddScoped<ITechnicianAvailabilityRepository, TechnitianAvailabilityRepository>();
            services.AddScoped<IAvailabilityExceptionRepository, AvailabilityExceptionRepository>();
            services.AddScoped<ITechnicianPricingRepository, TechnicianPricingRepository>();
            services.AddScoped<ITechnicianPhotoRepository, TechnicianPhotoRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>(); // ← add this


            services.AddScoped<IPromoCodeRepository, PromoCodeRepository>();
            services.AddScoped<IPromoCodeUsageRepository, PromoCodeUsageRepository>();
            //services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            //services.AddScoped<INewsletterRepository, NewsletterRepository>();

            services.AddScoped<ICategoryRepository, CategoriesRepository>();
            services.AddScoped<IProblemTypeRepository, ProblemTypesRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IDisputeRepository, DisputeRepository>();
            //services.AddScoped<IReviewAnalysisRepository, ReviewAnalysisRepository>();
            services.AddScoped<IHistoricalPriceRepository, HistoricalPriceRepository>();

            //services.AddScoped<IConversationRepository, ConversationRepository>();
            //services.AddScoped<IMessageRepository, MessageRepository>();
            //services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
            services.AddScoped<ISupportMessageRepository, SupportMessageRepository>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
             configuration.GetConnectionString("DefaultConnection"),
                o => o.UseNetTopologySuite()
                ));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
