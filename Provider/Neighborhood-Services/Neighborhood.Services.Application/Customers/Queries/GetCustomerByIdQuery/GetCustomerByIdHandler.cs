using MediatR;
using Neighborhood.Services.Application.Customers.DTOs;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Domain.Customers;

namespace Neighborhood.Services.Application.Customers.Queries
{
    public class GetCustomerByIdHandler(ICustomerRepository customerRepository) : IRequestHandler<GetCustomerByIdQuery, CustomerDetailsDTO>
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;

        public async Task<CustomerDetailsDTO> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(request.Id);
            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found");
            }

            return MapToDetails(customer);
        }

        private static CustomerDetailsDTO MapToDetails(Customer customer)
        {
            return new CustomerDetailsDTO
            {
                Id = customer.Id,
                ApplicationUserId = customer.ApplicationUserId,
                IsDeleted = customer.IsDeleted,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };
        }
    }
}
