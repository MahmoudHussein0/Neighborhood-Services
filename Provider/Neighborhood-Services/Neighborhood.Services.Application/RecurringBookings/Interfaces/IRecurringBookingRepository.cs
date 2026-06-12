using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.RecurringBookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.RecurringBookings.Interfaces
{
    public interface IRecurringBookingRepository: IGenericRepository<RecurringBooking,int>
    {
        Task<IEnumerable<RecurringBooking>> GetCustomerRecurringBookingsAsync(int customerId);
        Task<IEnumerable<RecurringBooking>> GetActiveRecurringBookingsAsync();

        // Paged + optional status filter + search (used by "my recurring bookings")
        Task<PagedResult<RecurringBooking>> GetCustomerRecurringBookingsPagedAsync(int customerId, RecurringBookingStatus? status, string? search, int page, int pageSize);
        Task<PagedResult<RecurringBooking>> GetTechnicianRecurringBookingsPagedAsync(int technicianId, RecurringBookingStatus? status, string? search, int page, int pageSize);
        Task<RecurringBooking?> GetRecurringBookingWithDetailsAsync(int recurringBookingId);
        Task<IEnumerable<RecurringBooking>> GetTechnicianRecurringBookingsAsync(int technicianId);

        Task<IEnumerable<RecurringBooking>> GetDueRecurringBookingsAsync(DateOnly date);
    }
}
