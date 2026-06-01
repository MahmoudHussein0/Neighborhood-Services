using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.CustomerAddresses.Interfaces;
using Neighborhood.Services.Domain.CustomerAddresses;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.CustomerAddresses
{
    public class CustomerAddressRepository : GenericRepository<CustomerAddress, int>, ICustomerAddressRepository
    {
        public CustomerAddressRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(CustomerAddress customerAddress)
        {
            if (customerAddress.IsDefault)
            {
                await ClearDefaultAddressesAsync(customerAddress.CustomerId);
            }

            await _context.CustomerAddresses.AddAsync(customerAddress);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(CustomerAddress customerAddress)
        {
            _context.CustomerAddresses.Update(customerAddress);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CustomerAddress>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.CustomerAddresses
                .AsNoTracking()
                .Where(customerAddress => customerAddress.CustomerId == customerId && !customerAddress.IsDeleted)
                .OrderByDescending(customerAddress => customerAddress.IsDefault)
                .ThenByDescending(customerAddress => customerAddress.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<CustomerAddress>> GetByUserIdAsync(string applicationUserId)
        {
            return await _context.CustomerAddresses
                .AsNoTracking()
                .Where(customerAddress => customerAddress.ApplicationUserId == applicationUserId && !customerAddress.IsDeleted)
                .OrderByDescending(customerAddress => customerAddress.IsDefault)
                .ThenByDescending(customerAddress => customerAddress.CreatedAt)
                .ToListAsync();
        }

        public async Task<CustomerAddress?> GetDefaultByCustomerIdAsync(int customerId)
        {
            return await _context.CustomerAddresses
                .AsNoTracking()
                .FirstOrDefaultAsync(customerAddress =>
                    customerAddress.CustomerId == customerId &&
                    customerAddress.IsDefault &&
                    !customerAddress.IsDeleted);
        }

        public async Task<List<CustomerAddress>> GetDeletedAsync()
        {
            return await _context.CustomerAddresses
                .AsNoTracking()
                .Where(customerAddress => customerAddress.IsDeleted)
                .OrderByDescending(customerAddress => customerAddress.CreatedAt)
                .ToListAsync();
        }

        public async Task SetDefaultAsync(CustomerAddress customerAddress)
        {
            await ClearDefaultAddressesAsync(customerAddress.CustomerId);

            customerAddress.IsDefault = true;
            customerAddress.IsDeleted = false;
            _context.CustomerAddresses.Update(customerAddress);

            await _context.SaveChangesAsync();
        }

        private async Task ClearDefaultAddressesAsync(int customerId)
        {
            var defaultAddresses = await _context.CustomerAddresses
                .Where(customerAddress => customerAddress.CustomerId == customerId && customerAddress.IsDefault)
                .ToListAsync();

            foreach (var defaultAddress in defaultAddresses)
            {
                defaultAddress.IsDefault = false;
            }
        }
    }
}
