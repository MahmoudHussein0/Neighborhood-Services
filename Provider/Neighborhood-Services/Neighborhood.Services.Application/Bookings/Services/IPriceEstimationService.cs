using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Bookings.Services
{
    public interface IPriceEstimationService
    {
        Task<decimal> EstimateAsync(int problemTypeId);

    }
}
