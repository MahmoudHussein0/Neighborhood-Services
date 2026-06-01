using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ServiceRequests.DTOs
{
    public class ServiceRequestWithOffersDto :ServiceRequestDetailsDto
    {
        public List<OfferSummaryDto> Offers { get; set; } = new();
    }
}
