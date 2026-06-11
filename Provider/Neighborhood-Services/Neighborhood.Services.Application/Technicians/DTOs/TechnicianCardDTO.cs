using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Application.Technicians.DTOs
{
    // Customer-facing "find technician" card. Joins Technician with its ApplicationUser
    // (name/photo/location) and its categories. Additive — does not replace TechnicianSummaryDTO.
    public class TechnicianCardDTO
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public string Experience { get; set; } = string.Empty;
        public int MaxTravelDistance { get; set; }
        public TechnicianVerificationStatus VerificationStatus { get; set; }
        public bool IsAvailable { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public List<TechnicianCardCategoryDTO> Categories { get; set; } = new();
    }

    public class TechnicianCardCategoryDTO
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
