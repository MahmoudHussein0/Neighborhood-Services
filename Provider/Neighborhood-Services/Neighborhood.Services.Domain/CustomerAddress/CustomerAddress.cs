using Neighborhood.Services.Domain.Customers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.CustomerAddresses
{
    public class CustomerAddress
    {
        public int Id { get; set; }
        public CustomerAddressLabel Label { get; set; }
        public string Address { get; set; } = string.Empty;
        public Point Location { get; set; } = null!;
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }


        public string ApplicationUserId { get; set; } = string.Empty;


        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
    }
}
