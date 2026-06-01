using MediatR;
using Neighborhood.Services.Application.CustomerAddresses.DTOs;

namespace Neighborhood.Services.Application.CustomerAddresses.Queries
{
    public class GetDeletedCustomerAddressesQuery : IRequest<List<CustomerAddressDTO>>
    {
    }
}
