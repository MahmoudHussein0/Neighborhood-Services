using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Domain.Customers;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;


namespace Neighborhood.Services.Infrastructure.Persistence.Customers
{
    public class CustomerRepository : GenericRepository<Customer, int>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
