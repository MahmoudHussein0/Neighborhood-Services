using Neighborhood.Services.Domain.AvilabilitiesException;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.favorites;
using Neighborhood.Services.Domain.Invoices;
using Neighborhood.Services.Domain.Offers;
using Neighborhood.Services.Domain.RecurringBookings;
using Neighborhood.Services.Domain.TechnicianPhotos;
using Neighborhood.Services.Domain.TechnicionCategories;
using Neighborhood.Services.Domain.TechnicionsAvailability;
using Neighborhood.Services.Domain.TechnicionsPricing;

namespace Neighborhood.Services.Domain.Technicians
{
    public class Technician
    {
        public int Id { get; set; }
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

        public string ApplicationUserId { get; set; } = string.Empty;

        public ICollection<TechnicianPhoto> TechnicianPhotos { get; set; } = new List<TechnicianPhoto>();
        public ICollection<TechnicianAvailability> TechnicianAvailabilities { get; set; } = new List<TechnicianAvailability>();
        public ICollection<AvailabilityException> AvailabilityExceptions { get; set; } = new List<AvailabilityException>();
        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<TechnicianCategory> TechnicianCategories { get; set; } = new List<TechnicianCategory>();
        public ICollection<TechnicianPricing> TechnicianPricings { get; set; } = new List<TechnicianPricing>();
        public ICollection<RecurringBooking> RecurringBookings { get; set; } = new List<RecurringBooking>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
