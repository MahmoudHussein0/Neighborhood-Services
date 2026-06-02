using MediatR;
using Neighborhood.Services.Application.CustomerAddresses.Interfaces;
using Neighborhood.Services.Domain.CustomerAddresses;
using NetTopologySuite.Geometries;

namespace Neighborhood.Services.Application.CustomerAddresses.Commands
{
    public class CreateCustomerAddressCommandHandler(ICustomerAddressRepository customerAddressRepository) : IRequestHandler<CreateCustomerAddressCommand, int>
    {
        private readonly ICustomerAddressRepository _customerAddressRepository = customerAddressRepository;

        public async Task<int> Handle(CreateCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            var customerAddress = new CustomerAddress
            {
                Label = request.Label,
                Address = request.Address,
                Location = new Point(request.Longitude, request.Latitude) { SRID = 4326 },
                IsDefault = request.IsDefault,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                ApplicationUserId = request.ApplicationUserId,
                CustomerId = request.CustomerId
            };

            await _customerAddressRepository.CreateAsync(customerAddress);
            return customerAddress.Id;
        }
    }
}
