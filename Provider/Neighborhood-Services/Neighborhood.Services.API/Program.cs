
using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.API.Middlewares;
using Neighborhood.Services.Application;
using Neighborhood.Services.Infrastructure;
using Neighborhood.Services.Infrastructure.Persistence.Context;

namespace Neighborhood.Services.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
                      builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddHttpContextAccessor();




            var app = builder.Build();

            //Arwa///

            //Mapping Notification Hub
            app.MapHub<Neighborhood.Services.Application.Notifications.Services.NotificationHub>("/notificationHub");
            //app.MapHub<ChatHub>("/chattt");
            //app.MapHub<NotificationHub>("/notf");

            //END OF ARWA

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseExceptionHandler();
            app.UseAuthorization();

           

            app.MapControllers();

            app.Run();
        }
    }
}
