using MediatR;
using Neighborhood.Services.Application.AiAnalysises.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AiAnalysises.Commands.AnalyzeBooking
{
    public class AnalyzeBookingCommand : IRequest<AiAnalysisDto>
    {
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int ProblemTypeId { get; set; }
        public int? BookingId { get; set; } // optional — link to booking if one exists
    }
}
