using MediatR;
using Neighborhood.Services.Application.CustomerAddresses.DTOs;

namespace Neighborhood.Services.Application.CustomerAddresses.Queries
{
    public class GetDefaultCustomerAddressQuery : IRequest<CustomerAddressDTO>
    {
        public int CustomerId { get; set; }
    }
}
