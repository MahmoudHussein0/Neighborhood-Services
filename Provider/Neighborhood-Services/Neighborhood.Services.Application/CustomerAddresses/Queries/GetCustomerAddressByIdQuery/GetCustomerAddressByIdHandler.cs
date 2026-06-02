using MediatR;
using Neighborhood.Services.Application.CustomerAddresses.DTOs;
using Neighborhood.Services.Application.CustomerAddresses.Interfaces;
using Neighborhood.Services.Domain.CustomerAddresses;

namespace Neighborhood.Services.Application.CustomerAddresses.Queries
{
    public class GetCustomerAddressByIdHandler(ICustomerAddressRepository customerAddressRepository) : IRequestHandler<GetCustomerAddressByIdQuery, CustomerAddressDTO>
    {
        private readonly ICustomerAddressRepository _customerAddressRepository = customerAddressRepository;

        public async Task<CustomerAddressDTO> Handle(GetCustomerAddressByIdQuery request, CancellationToken cancellationToken)
        {
            var customerAddress = await _customerAddressRepository.GetByIdAsync(request.Id);
            if (customerAddress == null)
            {
                throw new KeyNotFoundException("Customer address not found");
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
