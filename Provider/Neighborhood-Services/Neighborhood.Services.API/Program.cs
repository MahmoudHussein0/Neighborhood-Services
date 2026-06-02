
using Neighborhood.Services.API.Middlewares;
using Neighborhood.Services.Application;
using Neighborhood.Services.Infrastructure;
using Neighborhood.Services.Infrastructure.Persistence.Seeding;

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

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
                      builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddHttpContextAccessor();


            var app = builder.Build();

            // Seed dev/test data on startup (migrates + seeds if empty)
            using (var scope = app.Services.CreateScope())
            {
                await DbSeeder.SeedAsync(scope.ServiceProvider);
            }

            // Configure the HTTP request pipeline.
            if(app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseExceptionHandler();
            app.UseAuthorization();

            //app.MapHub<ChatHub>("/chattt");
            //app.MapHub<NotificationHub>("/notf");

            app.MapControllers();

            app.Run();
        }
    }
}
