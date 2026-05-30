using MediatR;
using Neighborhood.Services.Domain.Bookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Bookings.Commands.CreateBookingCommands
{
    public class CreateBookingCommand : IRequest<int>
    {
        public int CustomerId { get; set; }
        public int ProblemTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime ScheduledAt { get; set; }
        public BookingType BookingType { get; set; }
        public int? TechnicianId { get; set; }
        public int? PromoCodeId { get; set; }
    }
}
