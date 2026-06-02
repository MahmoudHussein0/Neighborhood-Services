using MediatR;
using Neighborhood.Services.Application.Customers.DTOs;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Domain.Customers;

namespace Neighborhood.Services.Application.Customers.Queries
{
    public class GetDeletedCustomersHandler(ICustomerRepository customerRepository) : IRequestHandler<GetDeletedCustomersQuery, List<CustomerSummaryDTO>>
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;

        public async Task<List<CustomerSummaryDTO>> Handle(GetDeletedCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.GetDeletedAsync();

            return customers.Select(MapToSummary).ToList();
        }

        private static CustomerSummaryDTO MapToSummary(Customer customer)
        {
            return new CustomerSummaryDTO
            {
                Id = customer.Id,
                ApplicationUserId = customer.ApplicationUserId,
                IsActive = customer.IsActive
            };
        }
    }
}
