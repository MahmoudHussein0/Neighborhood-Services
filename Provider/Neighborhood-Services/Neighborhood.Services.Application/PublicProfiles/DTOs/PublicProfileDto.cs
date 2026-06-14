namespace Neighborhood.Services.Application.PublicProfiles.DTOs
{
    // Public-facing profile shown when a customer clicks a technician (or a technician clicks a customer).
    // Symmetric for both roles; the technician-only fields stay empty for customers.
    public class PublicProfileDto
    {
        public string Role { get; set; } = string.Empty;          // "Technician" | "Customer"
        public string ApplicationUserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime MemberSince { get; set; }

        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int CompletedJobs { get; set; }

        // Technician-only extras (null/empty for customers)
        public string? Experience { get; set; }
        public string? VerificationStatus { get; set; }
        public List<PublicProfileCategoryDto> Categories { get; set; } = new();

        public List<PublicReviewDto> Reviews { get; set; } = new();
    }

    public class PublicReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public string ReviewerPhoto { get; set; } = string.Empty;
    }

    public class PublicProfileCategoryDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
