using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.CustomerAddresses;
using Neighborhood.Services.Domain.favorites;
using Neighborhood.Services.Domain.Invoices;
using Neighborhood.Services.Domain.RecurringBookings;
using Neighborhood.Services.Domain.ServiceRequests;
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
        public ICollection<Favorite> Favorites { get; set; } 
            = new List<Favorite>();
        public ICollection<Booking> Bookings { get; set; } 
            = new List<Booking>();
        public ICollection<RecurringBooking> RecurringBookings { get; set; } 
            = new List<RecurringBooking>();
        public ICollection<Invoice> Invoices { get; set; } =
            new List<Invoice>();
        public ICollection<ServiceRequest> ServiceRequests { get; set; } =
            new List<ServiceRequest>();

    }
}
