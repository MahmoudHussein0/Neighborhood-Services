using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;

namespace Neighborhood.Services.Application.Customers.Commands
{
    public class DeactivateCustomerCommandHandler(ICustomerRepository customerRepository) : IRequestHandler<DeactivateCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;

        public async Task Handle(DeactivateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(request.Id);
            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found");
            }

            customer.IsActive = false;
            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
        }
    }
}
