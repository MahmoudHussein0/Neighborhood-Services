    using Hangfire;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.SemanticKernel;
    using Neighborhood.Services.Application.AgentLogs.Interfaces;
    using Neighborhood.Services.Application.AI.Interfaces;
    using Neighborhood.Services.Application.AiAnalysises.Interface;
    using Neighborhood.Services.Application.AvilabilitiesException.Interfaces;
    using Neighborhood.Services.Application.BookingImages.Interface;
    using Neighborhood.Services.Application.Bookings.Interface;
    using Neighborhood.Services.Application.Cache;
    using Neighborhood.Services.Application.CancellationPolicies.Interfaces;
    using Neighborhood.Services.Application.Categories.Interfaces;
    using Neighborhood.Services.Application.Chatbot.Interfaces;
    using Neighborhood.Services.Application.Cloudinary;
    using Neighborhood.Services.Application.Conversations;
    using Neighborhood.Services.Application.CustomerAddresses.Interfaces;
    using Neighborhood.Services.Application.Customers.Interfaces;
    using Neighborhood.Services.Application.Disputes.Interfaces;
    using Neighborhood.Services.Application.Escrows.Interfaces;
    using Neighborhood.Services.Application.Favorites;
    using Neighborhood.Services.Application.HistoricalPrices.Interfaces;
    using Neighborhood.Services.Application.Invoices.Interfaces;
    using Neighborhood.Services.Application.Invoices.Services;
    using Neighborhood.Services.Application.Messages;
    using Neighborhood.Services.Application.Newsletter;
    using Neighborhood.Services.Application.Notifications;
    using Neighborhood.Services.Application.Notifications.Services;
    using Neighborhood.Services.Application.Offers.Interfaces;
    using Neighborhood.Services.Application.Payments.Gateways;
    using Neighborhood.Services.Application.Payments.Interfaces;
    using Neighborhood.Services.Application.ProblemTypes.Interface;
    using Neighborhood.Services.Application.PromoCodes.Interface;
using Neighborhood.Services.Application.QA.Interface;
using Neighborhood.Services.Application.RecurringBookings.Interfaces;
    using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.ReviewsAnalysis;
    using Neighborhood.Services.Application.ServiceRequests.Interfaces;
    using Neighborhood.Services.Application.Shared;
    using Neighborhood.Services.Application.Shared.Email;
    using Neighborhood.Services.Application.Staffs.Interfaces;
    using Neighborhood.Services.Application.SupportTickets.Interfaces;
    using Neighborhood.Services.Application.TechnicianPhotos.Interfaces;
    using Neighborhood.Services.Application.Technicians.Interfaces;
    using Neighborhood.Services.Application.TechnitianAvailability.Interfaces;
    using Neighborhood.Services.Application.TechnitianCategory.Interface;
    using Neighborhood.Services.Application.TechnitianPricing.Interface;
    using Neighborhood.Services.Application.Transactions.Interfaces;
    using Neighborhood.Services.Application.Users.Interfaces;
    using Neighborhood.Services.Application.Wallets.Interfaces;
    using Neighborhood.Services.Domain.ApplicationUsers;
    using Neighborhood.Services.Infrastructure.Cache;
    using Neighborhood.Services.Infrastructure.Persistence.AgentLogs;
    using Neighborhood.Services.Infrastructure.Persistence.AiAnalysises;
    using Neighborhood.Services.Infrastructure.Persistence.AvilabilitiesException;
    using Neighborhood.Services.Infrastructure.Persistence.BookingImages;
    using Neighborhood.Services.Infrastructure.Persistence.Bookings;
    using Neighborhood.Services.Infrastructure.Persistence.CancellationPolicies;
    using Neighborhood.Services.Infrastructure.Persistence.Categories;
    using Neighborhood.Services.Infrastructure.Persistence.Chatbot;
    using Neighborhood.Services.Infrastructure.Persistence.Context;
    using Neighborhood.Services.Infrastructure.Persistence.Conversations;
    using Neighborhood.Services.Infrastructure.Persistence.CustomerAddresses;
    using Neighborhood.Services.Infrastructure.Persistence.Customers;
    using Neighborhood.Services.Infrastructure.Persistence.Disputes.Repository;
    using Neighborhood.Services.Infrastructure.Persistence.Escrows;
    using Neighborhood.Services.Infrastructure.Persistence.Favorites;
    using Neighborhood.Services.Infrastructure.Persistence.HistoricalPrices;
    using Neighborhood.Services.Infrastructure.Persistence.Invoices;
    using Neighborhood.Services.Infrastructure.Persistence.Messages;
    using Neighborhood.Services.Infrastructure.Persistence.Newsletters;
    using Neighborhood.Services.Infrastructure.Persistence.Notifications;
    using Neighborhood.Services.Infrastructure.Persistence.Offers;
    using Neighborhood.Services.Infrastructure.Persistence.Payments;
    using Neighborhood.Services.Infrastructure.Persistence.ProblemTypes;
    using Neighborhood.Services.Infrastructure.Persistence.PromoCodes;
using Neighborhood.Services.Infrastructure.Persistence.QA;
using Neighborhood.Services.Infrastructure.Persistence.RecurringBookings;
    using Neighborhood.Services.Infrastructure.Persistence.Reviews.Repository;
using Neighborhood.Services.Infrastructure.Persistence.ReviewsAnalysis;
    using Neighborhood.Services.Infrastructure.Persistence.Seeding.Knowledge;
    using Neighborhood.Services.Infrastructure.Persistence.ServiceRequests;
    using Neighborhood.Services.Infrastructure.Persistence.Staffs.Repository;
    using Neighborhood.Services.Infrastructure.Persistence.SupportTickets.Repository;
    using Neighborhood.Services.Infrastructure.Persistence.TechnicianCategories;
    using Neighborhood.Services.Infrastructure.Persistence.TechnicianPhotos;
    using Neighborhood.Services.Infrastructure.Persistence.Technicians;
    using Neighborhood.Services.Infrastructure.Persistence.TechnitianAvailability;
    using Neighborhood.Services.Infrastructure.Persistence.TechnitianPricing;
    using Neighborhood.Services.Infrastructure.Persistence.Transactions;
    using Neighborhood.Services.Infrastructure.Persistence.Users;
    using Neighborhood.Services.Infrastructure.Persistence.Wallets;
    using Neighborhood.Services.Infrastructure.Services;
    using Neighborhood.Services.Infrastructure.Services.AI;
    using Neighborhood.Services.Infrastructure.Services.Authorization;
    using Neighborhood.Services.Infrastructure.Services.CloudinaryService;
    using Neighborhood.Services.Infrastructure.Services.EmailService;
    using Neighborhood.Services.Infrastructure.Services.Invoices;
    using Neighborhood.Services.Infrastructure.Services.NotificationService;
    using Neighborhood.Services.Infrastructure.Services.Payments;
    using Neighborhood.Services.Infrastructure.Shared;
    using Qdrant.Client;




namespace Neighborhood.Services.Infrastructure
    {
        public static class DependencyInjection
        {
            public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"),
                        o => o.UseNetTopologySuite()));

                services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

                services.AddScoped<IUnitOfWork, UnitOfWork>();

                services.AddScoped<IBookingRepository, BookingRepository>();
                services.AddScoped<IBookingImageRepository, BookingImageRepository>();
                services.AddScoped<IAiAnalysisRepository, AiAnalysisRepository>();
                services.AddScoped<IAgentLogRepository, AgentLogRepository>();
                services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
                services.AddScoped<IOfferRepository, OfferRepository>();
                services.AddScoped<IRecurringBookingRepository, RecurringBookingRepository>();
                services.AddScoped<ICancellationPolicyRepository, CancellationPolicyRepository>();
                services.AddScoped<IUserRepository, UserRepository>();

                services.AddScoped<IWalletRepository, WalletRepository>();
                services.AddScoped<ITransactionRepository, TransactionRepository>();
                services.AddScoped<IEscrowRepository, EscrowRepository>();
                services.AddScoped<IPaymentRepository, PaymentRepository>();
                services.AddScoped<IInvoiceRepository, InvoiceRepository>();

                services.AddScoped<ITechnicianCategoryRepository, TechnicianCategoryRepository>();
                services.AddScoped<ITechnicianRepository, TechnicianRepository>();
                services.AddScoped<ITechnicianAvailabilityRepository, TechnitianAvailabilityRepository>();
                services.AddScoped<IAvailabilityExceptionRepository, AvailabilityExceptionRepository>();
                services.AddScoped<ITechnicianPricingRepository, TechnicianPricingRepository>();
                services.AddScoped<ITechnicianPhotoRepository, TechnicianPhotoRepository>();
                services.AddScoped<ITechnicianCategoryRepository,TechnicianCategoryRepository>();
                services.AddScoped<ICustomerRepository, CustomerRepository>();
                services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
                services.AddScoped<IStaffRepository, StaffRepository>();

                services.AddScoped<IPromoCodeRepository, PromoCodeRepository>();
                services.AddScoped<IPromoCodeUsageRepository, PromoCodeUsageRepository>();
                //services.AddScoped<IFavoriteRepository, FavoriteRepository>();
                services.AddScoped<INewsletterRepository, NewsletterRepository>();

                services.AddScoped<ICategoryRepository, CategoriesRepository>();

                services.AddScoped<IProblemTypeRepository, ProblemTypesRepository>();
                services.AddScoped<IReviewRepository, ReviewRepository>();
                services.AddScoped<IReviewAnalysisRepository, ReviewAyalysisRepository>();
                services.AddScoped<IDisputeRepository, DisputeRepository>();
                //services.AddScoped<IReviewAnalysisRepository, ReviewAnalysisRepository>();
                services.AddScoped<IHistoricalPriceRepository, HistoricalPriceRepository>();

                //Arwa's
                services.AddScoped<IConversationRepository, ConversationRepository>();
                services.AddScoped<IMessageRepository, MessageRepository>();
                services.AddScoped<INotificationsRepository, NotificationsRepoisitory>();
                services.AddScoped<INewsletterRepository, NewsletterRepository>();
                services.AddScoped<IFavoritesRepository, FavoritesRepository>();
                services.AddScoped<IEmailService, EmailService>();
                services.AddScoped<INotificationService, NotificationService>();
            

                services.AddSingleton<IResponseCacheService, ResponseCacheService>();

                services.Configure<EmailConfiguration>(configuration.GetSection("EmailSettings"));
                //End of Arwa's

           
                services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
                services.AddScoped<ISupportMessageRepository, SupportMessageRepository>();


                services.AddScoped<ICurrentUserService, CurrentUserService>();
                services.AddScoped<IJwtTokenService, JwtTokenService>();
                services.AddScoped<IInvoicePdfService, InvoicePdfService>();
                services.AddHttpClient<IPaymentGatewayService, PaymentGatewayService>();
                services.Configure<PaymentGatewayOptions>(configuration.GetSection("PaymentGateway"));
                services.AddHttpClient<IGeocodingService, GeoapifyGeocodingService>(client =>
                {
                    client.BaseAddress = new Uri(configuration["Geoapify:BaseUrl"]!);
                });


                services.AddScoped<ITechnicianCategoryRepository, TechnicianCategoryRepository>();
                services.AddHangfire(config => config
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
                services.AddHangfireServer();
                services.AddScoped<RecurringBookingGeneratorService>();
                services.AddScoped<ServiceRequestExpiryService>();
                services.AddScoped<ServiceRequestModerationJob>();
                services.AddScoped<IBackgroundJobScheduler, BackgroundJobScheduler>();
                services.AddScoped<IKnowledgeIndexer, KnowledgeSeeder>();
                //Kernl
                services.AddSingleton(sp => {
                    var apiKey = configuration["OpenAI:ApiKey"] ?? "dummy-key";
                    return Kernel.CreateBuilder()
                        .AddOpenAIChatCompletion("gpt-4o", apiKey)
                        .Build();
                });
                services.AddScoped<IAiClient, SemanticKernelClient>();
                // AI 
                // --- Qdrant / RAG ---
                // 1- Embedding generator (text -> vector). Uses the same OpenAI key.
                var openAiKey = configuration["OpenAI:ApiKey"] ?? "dummy-key";
                    #pragma warning disable SKEXP0010
                     services.AddOpenAIEmbeddingGenerator("text-embedding-3-small", openAiKey);
                    #pragma warning restore SKEXP0010

                // 2- Qdrant client (talks to your cloud cluster over gRPC)
                var qdrantEndpoint = configuration["Qdrant:Endpoint"] ?? "https://dummy.qdrant.io";
                var qdrantApiKey = configuration["Qdrant:ApiKey"] ?? "dummy-key";
                services.AddSingleton(sp => new QdrantClient(
                    host: new Uri(qdrantEndpoint).Host,
                    https: true,
                    apiKey: qdrantApiKey));

                // 3- Our vector memory wrapper
                services.AddScoped<IVectorMemory, QdrantMemoryService>();
                // Chatbot
                services.AddScoped<IChatbotRepository, ChatbotRepository>();
                // QA  
                services.AddScoped<IQaAgent, QaAgent>();
                //Amira
                services.AddScoped<IAuthorizationHandler, PermissionHandler>();
                services.AddScoped<ICloudinaryService,CloudinaryService>();
                //end Amira
                return services;
            }
        }
    }
