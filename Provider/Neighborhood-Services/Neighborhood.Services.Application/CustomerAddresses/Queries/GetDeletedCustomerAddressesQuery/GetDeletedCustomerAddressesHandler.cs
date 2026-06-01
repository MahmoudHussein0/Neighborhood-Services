using MediatR;
using Neighborhood.Services.Application.CustomerAddresses.DTOs;
using Neighborhood.Services.Application.CustomerAddresses.Interfaces;
using Neighborhood.Services.Domain.CustomerAddresses;

namespace Neighborhood.Services.Application.CustomerAddresses.Queries
{
    public class GetDeletedCustomerAddressesHandler(ICustomerAddressRepository customerAddressRepository) : IRequestHandler<GetDeletedCustomerAddressesQuery, List<CustomerAddressDTO>>
    {
        private readonly ICustomerAddressRepository _customerAddressRepository = customerAddressRepository;

        public async Task<List<CustomerAddressDTO>> Handle(GetDeletedCustomerAddressesQuery request, CancellationToken cancellationToken)
        {
            var customerAddresses = await _customerAddressRepository.GetDeletedAsync();

            return customerAddresses.Select(MapToDto).ToList();
        }

        private static CustomerAddressDTO MapToDto(CustomerAddress customerAddress)
        {
            return new CustomerAddressDTO
            {
                Id = customerAddress.Id,
                Label = customerAddress.Label,
                Address = customerAddress.Address,
                Latitude = customerAddress.Location.Y,
                Longitude = customerAddress.Location.X,
                IsDefault = customerAddress.IsDefault,
                IsDeleted = customerAddress.IsDeleted,
                CreatedAt = customerAddress.CreatedAt,
                ApplicationUserId = customerAddress.ApplicationUserId,
                CustomerId = customerAddress.CustomerId
            };
        }
    }
}
