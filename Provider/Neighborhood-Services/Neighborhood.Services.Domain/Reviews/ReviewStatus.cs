using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.Reviews
{
    public enum ReviewStatus
    {
        // Just submitted — awaiting QaAgent moderation.
        Pending = 1,
        // QaAgent passed it — visible to the public.
        Approved = 2,
        // Customer/staff explicitly took it down (not used by the moderation pipeline).
        Rejected = 3,
        // QaAgent flagged it (abuse/spam/PII). Hidden from public, surfaces in the
        // staff "flagged reviews" queue alongside Analysis.IsFlagged = true.
        Flagged = 4
    }
}
