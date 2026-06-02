using MediatR;
using Neighborhood.Services.Application.CustomerAddresses.Interfaces;
using NetTopologySuite.Geometries;

namespace Neighborhood.Services.Application.CustomerAddresses.Commands
{
    public class UpdateCustomerAddressCommandHandler(ICustomerAddressRepository customerAddressRepository) : IRequestHandler<UpdateCustomerAddressCommand>
    {
        private readonly ICustomerAddressRepository _customerAddressRepository = customerAddressRepository;

        public async Task Handle(UpdateCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            var customerAddress = await _customerAddressRepository.GetByIdAsync(request.Id);
            if (customerAddress == null)
            {
                throw new KeyNotFoundException("Customer address not found");
            }

            customerAddress.Label = request.Label;
            customerAddress.Address = request.Address;
            customerAddress.Location = new Point(request.Longitude, request.Latitude) { SRID = 4326 };

            await _customerAddressRepository.UpdateAsync(customerAddress);
        }
    }
}
