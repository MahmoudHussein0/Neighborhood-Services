namespace Neighborhood.Services.Application.Customers.DTOs
{
    public class CustomerDetailsDTO
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
