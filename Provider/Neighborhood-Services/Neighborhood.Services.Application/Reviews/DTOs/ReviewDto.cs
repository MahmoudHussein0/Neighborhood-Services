namespace Neighborhood.Services.Application.Reviews.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string ReviewerId { get; set; }
        public string RevieweeId { get; set; }
        public string? ReviewerName { get; set; }
        public string? RevieweeName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
