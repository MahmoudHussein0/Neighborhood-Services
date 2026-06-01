using MediatR;
using Neighborhood.Services.Application.CustomerAddresses.DTOs;
using Neighborhood.Services.Application.CustomerAddresses.Interfaces;
using Neighborhood.Services.Domain.CustomerAddresses;

namespace Neighborhood.Services.Application.CustomerAddresses.Queries
{
    public class GetDefaultCustomerAddressHandler(ICustomerAddressRepository customerAddressRepository) : IRequestHandler<GetDefaultCustomerAddressQuery, CustomerAddressDTO>
    {
        private readonly ICustomerAddressRepository _customerAddressRepository = customerAddressRepository;

        public async Task<CustomerAddressDTO> Handle(GetDefaultCustomerAddressQuery request, CancellationToken cancellationToken)
        {
            var customerAddress = await _customerAddressRepository.GetDefaultByCustomerIdAsync(request.CustomerId);
            if (customerAddress == null)
            {
                throw new KeyNotFoundException("Default customer address not found");
            }

            return MapToDto(customerAddress);
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
