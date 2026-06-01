using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;

namespace Neighborhood.Services.Application.Customers.Commands
{
    public class ActivateCustomerCommandHandler(ICustomerRepository customerRepository) : IRequestHandler<ActivateCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;

        public async Task Handle(ActivateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(request.Id);
            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found");
            }

            customer.IsActive = true;
            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
        }
    }
}
