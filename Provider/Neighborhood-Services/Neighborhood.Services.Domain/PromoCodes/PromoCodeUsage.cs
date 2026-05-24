namespace Neighborhood.Services.Domain.PromoCodes
{
    public class PromoCodeUsage
    {
        public int Id { get; set; }
        public int PromoCodeId { get; set; }
        public int UserId { get; set; }
        public int BookingId { get; set; }
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
        public PromoCode PromoCode { get; set; } = null!;
    }
}