using Microsoft.Extensions.DependencyInjection;
using Neighborhood.Services.Application.Bookings.Services;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // MediatR
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            // FluentValidation
            //services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            // Mapster
            //services.AddMapster();
            services.AddScoped<IPriceEstimationService, PriceEstimationService>();
            services.AddScoped<IRegionResolver, RegionResolver>();

            return services;
        }
    }
}
