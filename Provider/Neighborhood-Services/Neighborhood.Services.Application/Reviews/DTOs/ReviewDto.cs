namespace Neighborhood.Services.Application.Reviews.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int ReviewerId { get; set; }
        public int RevieweeId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
