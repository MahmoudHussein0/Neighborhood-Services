using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.Staffs
{
    public enum PermissionType
    {
        ManageDisputes = 1,
        ManageTickets = 2,
        ViewTransactions = 3,
        ManagePromos = 4,
        ApproveTechnicians = 5,
        FlagReviews = 6,
        ManageUsers = 7,
        FullAccess = 8
    }
}
