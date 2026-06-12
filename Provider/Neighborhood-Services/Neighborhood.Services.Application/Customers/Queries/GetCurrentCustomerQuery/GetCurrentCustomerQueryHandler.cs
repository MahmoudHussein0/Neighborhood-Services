using MediatR;
using Neighborhood.Services.Application.Customers.DTOs;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Customers;

namespace Neighborhood.Services.Application.Customers.Queries.GetCurrentCustomerQuery
{
    public class GetCurrentCustomerQueryHandler(
        ICurrentUserService currentUserService,
        ICustomerRepository customerRepository) : IRequestHandler<GetCurrentCustomerQuery, CustomerDetailsDTO>
    {
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly ICustomerRepository _customerRepository = customerRepository;

        public async Task<CustomerDetailsDTO> Handle(GetCurrentCustomerQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException("Current user is not authenticated");
            }

            var customer = await _customerRepository.GetByUserIdAsync(userId);
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
