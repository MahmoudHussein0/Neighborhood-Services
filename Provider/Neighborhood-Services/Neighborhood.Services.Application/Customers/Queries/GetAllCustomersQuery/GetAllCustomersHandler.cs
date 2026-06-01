using MediatR;
using Neighborhood.Services.Application.Customers.DTOs;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Domain.Customers;

namespace Neighborhood.Services.Application.Customers.Queries
{
    public class GetAllCustomersHandler(ICustomerRepository customerRepository) : IRequestHandler<GetAllCustomersQuery, List<CustomerSummaryDTO>>
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;

        public async Task<List<CustomerSummaryDTO>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.GetAllCustomersAsync();

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
