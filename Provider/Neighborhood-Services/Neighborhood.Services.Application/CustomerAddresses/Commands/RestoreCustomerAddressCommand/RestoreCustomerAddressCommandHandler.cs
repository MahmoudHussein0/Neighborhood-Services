using MediatR;
using Neighborhood.Services.Application.CustomerAddresses.Interfaces;

namespace Neighborhood.Services.Application.CustomerAddresses.Commands
{
    public class RestoreCustomerAddressCommandHandler(ICustomerAddressRepository customerAddressRepository) : IRequestHandler<RestoreCustomerAddressCommand>
    {
        private readonly ICustomerAddressRepository _customerAddressRepository = customerAddressRepository;

        public async Task Handle(RestoreCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            var customerAddress = await _customerAddressRepository.GetByIdAsync(request.Id);
            if (customerAddress == null)
            {
                throw new KeyNotFoundException("Customer address not found");
            }

            customerAddress.IsDeleted = false;

            await _customerAddressRepository.UpdateAsync(customerAddress);
        }
    }
}
