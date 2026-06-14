using MediatR;
using Neighborhood.Services.Application.PublicProfiles.DTOs;

namespace Neighborhood.Services.Application.Customers.Queries
{
    public class GetCustomerPublicProfileQuery : IRequest<PublicProfileDto>
    {
        public int CustomerId { get; set; }
    }
}
