using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Domain.Customers;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.Customers
{
    public class CustomerRepository : GenericRepository<Customer, int>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<Customer?> GetByUserIdAsync(string applicationUserId)
        {
            return await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(customer => customer.ApplicationUserId == applicationUserId && !customer.IsDeleted);
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(customer => !customer.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Customer>> GetActiveAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(customer => customer.IsActive && !customer.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Customer>> GetDeletedAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(customer => customer.IsDeleted)
                .ToListAsync();
        }
    }
}
