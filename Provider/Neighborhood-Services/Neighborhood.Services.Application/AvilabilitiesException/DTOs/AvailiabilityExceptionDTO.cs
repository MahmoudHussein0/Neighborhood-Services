using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AvilabilitiesException.DTOs
{
    public class AvailiabilityExceptionDTO
    {
        public int TechnicianId { get; set; }
        public DateOnly Date { get; set; }
        public bool IsAvailable { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string? Reason { get; set; }
    }
}
