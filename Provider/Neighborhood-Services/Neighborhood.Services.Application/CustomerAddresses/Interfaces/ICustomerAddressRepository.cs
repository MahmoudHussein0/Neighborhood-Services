using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.CustomerAddresses;

namespace Neighborhood.Services.Application.CustomerAddresses.Interfaces
{
    public interface ICustomerAddressRepository : IGenericRepository<CustomerAddress, int>
    {
        Task CreateAsync(CustomerAddress customerAddress);
        Task<List<CustomerAddress>> GetByCustomerIdAsync(int customerId);
        Task<List<CustomerAddress>> GetByUserIdAsync(string applicationUserId);
        Task<CustomerAddress?> GetDefaultByCustomerIdAsync(int customerId);
        Task<List<CustomerAddress>> GetDeletedAsync();
        Task SetDefaultAsync(CustomerAddress customerAddress);
    }
}
