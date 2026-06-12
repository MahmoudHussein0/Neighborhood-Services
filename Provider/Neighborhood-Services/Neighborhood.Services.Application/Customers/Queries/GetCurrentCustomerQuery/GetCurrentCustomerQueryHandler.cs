using MediatR;
using Neighborhood.Services.Application.Customers.DTOs;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Users.Interfaces;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Customers;

namespace Neighborhood.Services.Application.Customers.Queries.GetCurrentCustomerQuery
{
    public class GetCurrentCustomerQueryHandler(
        ICurrentUserService currentUserService,
        ICustomerRepository customerRepository,
        IUserRepository userRepository) : IRequestHandler<GetCurrentCustomerQuery, CustomerDetailsDTO>
    {
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly ICustomerRepository _customerRepository = customerRepository;
        private readonly IUserRepository _userRepository = userRepository;

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
                var user = await _userRepository.GetByIdAsync(userId)
                    ?? throw new NotFoundException("User not found");

                if (user.ApplicationUserRole != ApplicationUserRole.Customer)
                {
                    throw new ForbiddenException("The current user is not a customer");
                }

                customer = new Customer
                {
                    ApplicationUserId = userId,
                    IsDeleted = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _customerRepository.CreateAsync(customer);
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
