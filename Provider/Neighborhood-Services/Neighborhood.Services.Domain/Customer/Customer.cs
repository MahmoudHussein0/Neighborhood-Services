using Neighborhood.Services.Domain.CustomerAddresses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.Customers
{
    public class Customer
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public string ApplicationUserId { get; set; } = string.Empty;

        public ICollection<CustomerAddress> CustomerAddresses { get; set; } 
            = new List<CustomerAddress>();
    }
}
