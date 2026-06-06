using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Shared;
namespace Neighborhood.Services.Domain.PromoCodes
{

    public class PromoCode : BaseEntity<int>
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public int MaxUses { get; set; } = 1;
        public int UsedCount { get; set; } = 0;
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<PromoCodeUsage> Usages { get; set; } = new HashSet<PromoCodeUsage>();
        public ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
    }
}