using MediatR;
using Neighborhood.Services.Application.CustomerAddresses.Interfaces;

namespace Neighborhood.Services.Application.CustomerAddresses.Commands
{
    public class SetDefaultCustomerAddressCommandHandler(ICustomerAddressRepository customerAddressRepository) : IRequestHandler<SetDefaultCustomerAddressCommand>
    {
        private readonly ICustomerAddressRepository _customerAddressRepository = customerAddressRepository;

        public async Task Handle(SetDefaultCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            var customerAddress = await _customerAddressRepository.GetByIdAsync(request.Id);
            if (customerAddress == null)
            {
                throw new KeyNotFoundException("Customer address not found");
            }

            await _customerAddressRepository.SetDefaultAsync(customerAddress);
        }
    }
}
