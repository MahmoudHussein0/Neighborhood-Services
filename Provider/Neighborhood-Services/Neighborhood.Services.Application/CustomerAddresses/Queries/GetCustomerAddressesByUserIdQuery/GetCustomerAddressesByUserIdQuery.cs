using MediatR;
using Neighborhood.Services.Application.CustomerAddresses.DTOs;

namespace Neighborhood.Services.Application.CustomerAddresses.Queries
{
    public class GetCustomerAddressesByUserIdQuery : IRequest<List<CustomerAddressDTO>>
    {
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
