using Neighborhood.Services.Domain.Shared;
namespace Neighborhood.Services.Domain.PromoCodes
{
    public class PromoCode : BaseEntity<int>
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public int MaxUses { get; set; }
        public int UsedCount { get; set; } = 0;
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public ICollection<PromoCodeUsage> Usages { get; set; } = new List<PromoCodeUsage>();
    }
}