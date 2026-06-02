using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Application.Technicians.DTOs
{
    public class TechnicianDetailsDTO
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public int MaxTravelDistance { get; set; }
        public TechnicianVerificationStatus VerificationStatus { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
