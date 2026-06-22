namespace Neighborhood.Services.Application.Bookings.Enums
{
    // Sort options for the paged booking lists (staff oversight + customer/technician "mine").
    public enum BookingSortBy
    {
        NewestCreated = 0,    // CreatedAt desc (default)
        OldestCreated = 1,    // CreatedAt asc
        SoonestScheduled = 2, // ScheduledAt asc
        LatestScheduled = 3   // ScheduledAt desc
    }
}
