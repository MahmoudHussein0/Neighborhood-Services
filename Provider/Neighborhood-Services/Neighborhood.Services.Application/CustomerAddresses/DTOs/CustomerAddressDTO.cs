using Neighborhood.Services.Domain.CustomerAddresses;

namespace Neighborhood.Services.Application.CustomerAddresses.DTOs
{
    public class CustomerAddressDTO
    {
        public int Id { get; set; }
        public CustomerAddressLabel Label { get; set; }
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public int CustomerId { get; set; }
    }
}
