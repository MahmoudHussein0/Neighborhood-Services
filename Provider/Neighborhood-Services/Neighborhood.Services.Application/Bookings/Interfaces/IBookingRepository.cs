using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Bookings.Interface
{
    public interface IBookingRepository: IGenericRepository<Booking,int>
    {
        Task<Booking?> GetBookingWithDetailsAsync(int bookingId);
        Task<IEnumerable<Booking>> GetCustomerBookingsAsync(int customerId, bool includeDeleted = false);
        Task<IEnumerable<Booking>> GetTechnicianBookingsAsync(int technicianId);
        Task<IEnumerable<Booking>> GetAllBookingsAsync();  // admin
        Task<Booking?> GetBookingWithImagesAsync(int bookingId);
        Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status);

        Task<Booking?> GetActiveBookingForTechnicianAsync(int technicianId, DateTime scheduledAt);

        Task<Booking?> GetBookingWithEscrowAsync(int bookingId);
    }
}
