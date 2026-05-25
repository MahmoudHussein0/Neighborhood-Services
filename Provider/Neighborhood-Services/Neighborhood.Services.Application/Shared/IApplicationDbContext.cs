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


        //Arwa
        public DbSet<Message> Messages { get; }
        public DbSet<Conversation> Conversations { get;  }

        public DbSet<Notification> Notifications { get;  }
        public DbSet<Newsletter> Newsletters { get; }
        ////////////////////END OF ARWA //////////////////////
    }
}
