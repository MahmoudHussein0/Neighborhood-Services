using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;

namespace Neighborhood.Services.Application.Customers.Commands
{
    public class DeleteCustomerCommandHandler(ICustomerRepository customerRepository) : IRequestHandler<DeleteCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;

        public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(request.Id);
            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found");
            }

            customer.IsDeleted = true;
            customer.IsActive = false;
            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
        }
    }
}
