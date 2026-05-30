using Neighborhood.Services.Application.CustomerAddresses.Interfaces;
using Neighborhood.Services.Domain.CustomerAddresses;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.CustomerAddresses
{
    public class CustomerAddressRepository : GenericRepository<CustomerAddress, int>, ICustomerAddressRepository
    {
        public CustomerAddressRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
