using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Neighborhood.Services.API.Middlewares;
using Neighborhood.Services.Application;
using Neighborhood.Services.Infrastructure;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Persistence.Seeding;
using Neighborhood.Services.Infrastructure.Persistence.Seeding.Knowledge;
using Neighborhood.Services.Infrastructure.Services;
using System.Text;


namespace Neighborhood.Services.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter()));
            builder.Services.AddSignalR();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
                    };
                });
            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
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
            using (var scope = app.Services.CreateScope())
            {
                await DbSeeder.SeedAsync(scope.ServiceProvider);

                // Seed Qdrant knowledge base from JSON files
                var knowledgeSeeder = scope.ServiceProvider.GetRequiredService<KnowledgeSeeder>();
                await knowledgeSeeder.SeedAsync(app.Environment.ContentRootPath);
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseExceptionHandler();
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
