using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Bookings.Commands.CreateBookingCommands
{
    // This command handles Direct bookings only.
    // Bidding bookings are created internally via AcceptOfferCommand.
    public class CreateBookingCommand : IRequest<int>
    {
        // CustomerId is resolved from the authenticated user in the handler.
        public int TechnicianId { get; set; }
        public int ProblemTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string?  Region  { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int? PromoCodeId { get; set; }
        // Optional photo of the problem (already hosted on Cloudinary) the customer attaches
        // so the technician can see the current state before quoting. Saved as a "Before" image.
        public string? BeforeImageUrl { get; set; }
    }
}
