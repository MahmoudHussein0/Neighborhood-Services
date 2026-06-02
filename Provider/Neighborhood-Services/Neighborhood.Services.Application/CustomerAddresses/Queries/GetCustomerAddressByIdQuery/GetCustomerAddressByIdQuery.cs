using MediatR;
using Neighborhood.Services.Application.CustomerAddresses.DTOs;

namespace Neighborhood.Services.Application.CustomerAddresses.Queries
{
    public class GetCustomerAddressByIdQuery : IRequest<CustomerAddressDTO>
    {
        public int Id { get; set; }
    }
}
