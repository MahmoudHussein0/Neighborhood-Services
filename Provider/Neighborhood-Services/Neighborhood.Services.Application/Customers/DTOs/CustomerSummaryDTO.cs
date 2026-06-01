namespace Neighborhood.Services.Application.Customers.DTOs
{
    public class CustomerSummaryDTO
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
