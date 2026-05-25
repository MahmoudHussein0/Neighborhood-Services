using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Customers.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer, int>
    {
    }
}
