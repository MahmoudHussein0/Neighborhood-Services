using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Domain.Customers;

namespace Neighborhood.Services.Application.Customers.Commands
{
    public class CreateCustomerCommandHandler(ICustomerRepository customerRepository) : IRequestHandler<CreateCustomerCommand, int>
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;

        public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = new Customer
            {
                ApplicationUserId = request.ApplicationUserId,
                IsDeleted = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _customerRepository.CreateAsync(customer);
            return customer.Id;
        }
    }
}
