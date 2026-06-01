using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;

namespace Neighborhood.Services.Application.Customers.Commands
{
    public class RestoreCustomerCommandHandler(ICustomerRepository customerRepository) : IRequestHandler<RestoreCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;

        public async Task Handle(RestoreCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(request.Id);
            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found");
            }

            customer.IsDeleted = false;
            customer.IsActive = true;
            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
        }
    }
}
