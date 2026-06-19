namespace Neighborhood.Services.Application.PromoCodes.DTOs
{
    /// <summary>
    /// Lightweight, read-only result for previewing a promo code before it is applied.
    /// Tells the UI whether the current user can actually use the code (valid + not already used)
    /// and, if so, the discount percentage — without consuming the code.
    /// </summary>
    public class PromoCodePreviewDto
    {
        public bool IsApplicable { get; set; }
        public decimal DiscountPercentage { get; set; }

        /// <summary>null when applicable; otherwise "invalid_or_expired" or "already_used".</summary>
        public string? Reason { get; set; }
    }
}
