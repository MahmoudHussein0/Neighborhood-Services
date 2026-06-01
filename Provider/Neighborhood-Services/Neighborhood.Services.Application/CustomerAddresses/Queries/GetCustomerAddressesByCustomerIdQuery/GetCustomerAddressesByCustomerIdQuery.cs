using MediatR;
using Neighborhood.Services.Application.CustomerAddresses.DTOs;

namespace Neighborhood.Services.Application.CustomerAddresses.Queries
{
    public class GetCustomerAddressesByCustomerIdQuery : IRequest<List<CustomerAddressDTO>>
    {
        public int CustomerId { get; set; }
    }
}
