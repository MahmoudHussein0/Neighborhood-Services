using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Domain.AvilabilitiesException
{
    public class AvailabilityException :BaseEntity<int>
    {
        public int TechnicianId { get; set; }
        public DateOnly Date { get; set; }
        public bool IsAvailable { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public Technician Technician { get; set; }

    }
}
