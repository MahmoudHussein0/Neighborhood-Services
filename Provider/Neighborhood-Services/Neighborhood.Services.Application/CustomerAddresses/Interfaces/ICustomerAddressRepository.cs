using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.CustomerAddresses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.CustomerAddresses.Interfaces
{
    public interface ICustomerAddressRepository : IGenericRepository<CustomerAddress, int>
    {
    }
}
