using MediatR;
using Neighborhood.Services.Application.CustomerAddresses.Interfaces;

namespace Neighborhood.Services.Application.CustomerAddresses.Commands
{
    public class DeleteCustomerAddressCommandHandler(ICustomerAddressRepository customerAddressRepository) : IRequestHandler<DeleteCustomerAddressCommand>
    {
        private readonly ICustomerAddressRepository _customerAddressRepository = customerAddressRepository;

        public async Task Handle(DeleteCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            var customerAddress = await _customerAddressRepository.GetByIdAsync(request.Id);
            if (customerAddress == null)
            {
                throw new KeyNotFoundException("Customer address not found");
            }

            customerAddress.IsDeleted = true;
            customerAddress.IsDefault = false;

            await _customerAddressRepository.UpdateAsync(customerAddress);
        }
    }
}
