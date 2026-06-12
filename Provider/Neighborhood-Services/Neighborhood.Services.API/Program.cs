using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Neighborhood.Services.API.Middlewares;
using Neighborhood.Services.Application;
using Neighborhood.Services.Application.AI.Interfaces;
using Neighborhood.Services.Application.Authorization;
using Neighborhood.Services.Application.Cloudinary;
using Neighborhood.Services.Domain.Staffs;
using Neighborhood.Services.Infrastructure;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Persistence.Seeding;
using Neighborhood.Services.Infrastructure.Persistence.Seeding.Knowledge;
using Neighborhood.Services.Infrastructure.Services;
using Neighborhood.Services.Infrastructure.Services.CloudinaryService;
using StackExchange.Redis;
using System.Text;


namespace Neighborhood.Services.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //Email config: Moved to Infra
            //builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter()));
            builder.Services.AddSignalR();
            //builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Frontend", policy =>
                {
                    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

                    if (allowedOrigins.Length > 0)
                    {
                        policy.WithOrigins(allowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }
                });
            });


            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext =>
                {
                    var errors = actionContext.ModelState.Where(M => M.Value.Errors.Count() > 0)
                             .SelectMany(M => M.Value.Errors)
                             .Select(E => !(string.IsNullOrEmpty(E.Exception?.Message)) ? E.Exception.Message : E.ErrorMessage)
                             .ToArray();
                    return new BadRequestObjectResult(new
                    {
                        StatusCod = 400,
                        Errors = errors
                    });
                });
            });



            builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });


            builder.Services.AddAuthentication(options =>
            {
                // AddIdentity() sets the default authenticate scheme to the Identity cookie, which
                // means our JWT (in the access_token cookie) is never read. Force JwtBearer to be
                // the default for authenticate/challenge so protected endpoints actually use the JWT.
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
                        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var authorizationHeader = context.Request.Headers.Authorization.ToString();
                            if (string.IsNullOrWhiteSpace(authorizationHeader) ||
                                !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                context.Token = context.Request.Cookies["access_token"];
                            }

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddGoogle(options =>
                {
                    //options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
                    //options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "dummy-google-client-id";
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "dummy-google-client-secret";
                });
            // Add authorization policies for each permission type (Amira)
            builder.Services.AddAuthorization(options =>
            {
                foreach (PermissionType permission in Enum.GetValues(typeof(PermissionType)))
                {
                    options.AddPolicy(
                        $"Permission:{permission}",
                        policy => policy.Requirements.Add(
                            new PermissionRequirement(permission)));
                }
            });

            builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));

            builder.Services.AddScoped<ICloudinaryService,
                CloudinaryService>();
            // end of Amira
            //builder.Services.AddAuthorization();

            //builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                // Use full type names as schema IDs so two commands with the same class
                // name in different namespaces (e.g. UpdateTechnicianAvailabilityCommand
                // in both Technicians.Commands and TechnitianAvailability.Commands) don't
                // collide.
                c.CustomSchemaIds(type => type.FullName);
            });
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            builder.Services.AddHttpContextAccessor();





            var app = builder.Build();


            //Arwa///

            //Mapping Notification Hub
            app.MapHub<Neighborhood.Services.Infrastructure.Services.NotificationService.NotificationHub>("/notificationHub");
            //app.MapHub<ChatHub>("/chattt");
            //app.MapHub<NotificationHub>("/notf");

            //END OF ARWA
            // Seed dev/test data on startup (migrates + seeds if empty)
            // CLI: pure knowledge reindex. Assumes the DB is already migrated (boot the app
            // normally at least once first). Does NOT seed dev data, does NOT start the server.
            //   dotnet run -- reindex-knowledge   (same job as POST /api/knowledge/reindex)
            if (args.Contains("reindex-knowledge"))
            {
                using var reindexScope = app.Services.CreateScope();
                await reindexScope.ServiceProvider.GetRequiredService<IKnowledgeIndexer>().ReindexAllAsync();
                Console.WriteLine("Knowledge reindex complete.");
                return;
            }

            // Normal boot only: migrate + seed dev/test data. (Knowledge index is NOT seeded here —
            // rebuild it deliberately via the CLI above or POST /api/knowledge/reindex.)
            using (var scope = app.Services.CreateScope())
            {
                var environment = app.Services.GetRequiredService<IWebHostEnvironment>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                await DbSeeder.SeedAsync(scope.ServiceProvider, environment, logger);


                // Seed Qdrant knowledge base from the DB (catalog) + Faqs.json.
                // If OpenAI/Qdrant is unavailable (bad key, no quota, network down) we log
                // and continue — the app stays up; only the AI endpoints will fail per-call.
                try
                {
                    var knowledgeSeeder = scope.ServiceProvider.GetRequiredService<KnowledgeSeeder>();
                    await knowledgeSeeder.SeedAsync();
                }
                catch (Exception ex)
                {
                    var logger2 = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger2.LogWarning(ex, "KnowledgeSeeder failed at startup — AI endpoints may not work until this is fixed. App will continue to run.");
                }

            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            //app.UseCors("AllowJS");
            app.UseHttpsRedirection();
            app.UseExceptionHandler();
            app.UseStaticFiles();
            app.UseCors("Frontend");
           
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard("/hangfire");
            RecurringJob.AddOrUpdate<RecurringBookingGeneratorService>(
                "recurring-booking-generator",
                service => service.GenerateBookings(),
                Cron.Daily);
            RecurringJob.AddOrUpdate<ServiceRequestExpiryService>(
                "service_request_expiry",
                service => service.ExpireOpenRequestAndOffer(),
                Cron.Daily);

            app.MapControllers();

            app.Run();
        }
    }
}
