
using Neighborhood.Services.Domain.Disputes;
using Neighborhood.Services.Domain.Reviews;
using Neighborhood.Services.Domain.Staffs;
using Neighborhood.Services.Domain.SupportTickets;
using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Domain.Newsletter;
using Neighborhood.Services.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Shared
{
    public interface IApplicationDbContext
    {
        // Add DbSets for your entities here (get only)
        public DbSet<Review> Reviews { get; }

        public DbSet<ReviewAnalysis> ReviewAnalyses { get; }

        public DbSet<Staff> Staffs { get; }

        public DbSet<StaffPermission> StaffPermissions { get; }

        public DbSet<Dispute> Disputes { get; }

        public DbSet<Conversation> Conversations { get; }

        DbSet<SupportTicket> SupportTickets { get; }
        DbSet<SupportMessage> SupportMessages { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


        //Arwa
        public DbSet<Message> Messages { get; }
        public DbSet<Conversation> Conversations { get;  }

        public DbSet<Notification> Notifications { get;  }
        public DbSet<Newsletter> Newsletters { get; }
        ////////////////////END OF ARWA //////////////////////
    }
}
