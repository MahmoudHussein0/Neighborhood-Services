using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Shared;
namespace Neighborhood.Services.Domain.PromoCodes
{
    public class PromoCodeUsage : BaseEntity<int>
    {
        public int PromoCodeId { get; set; }
        public int UserId { get; set; }
        public int BookingId { get; set; }
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
        public PromoCode PromoCode { get; set; } = null!;
        public Booking Booking { get; set; } = null!;
    }
}