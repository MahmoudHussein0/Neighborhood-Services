using Neighborhood.Services.Application.PublicProfiles.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Customers;

namespace Neighborhood.Services.Application.Customers.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer, int>
    {
        Task CreateAsync(Customer customer);
        Task<Customer?> GetByUserIdAsync(string applicationUserId);
        Task<List<Customer>> GetAllCustomersAsync();
        Task<List<Customer>> GetActiveAsync();
        Task<List<Customer>> GetDeletedAsync();

        // Public profile (details + stats + approved reviews with reviewer name/photo) for a customer.
        Task<PublicProfileDto?> GetPublicProfileAsync(int customerId);
    }
}
