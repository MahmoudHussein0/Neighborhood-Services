using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.ServiceRequests
{
    public enum ServiceRequestStatus
    {
        Open,
        Closed,
        Expired,

        // Just created — waiting for the moderation agent to run in the background.
        // Not visible to technicians (browse queries filter on Open).
        PendingReview,

        // The moderation agent judged the description/image inappropriate.
        // Kept in the DB, hidden from technicians, surfaced to staff for review.
        Flagged

    }
}
