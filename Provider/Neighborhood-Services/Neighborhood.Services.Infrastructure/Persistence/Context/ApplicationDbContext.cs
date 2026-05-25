using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Disputes;
using Neighborhood.Services.Domain.Reviews;
using Neighborhood.Services.Domain.Staffs;
using Neighborhood.Services.Domain.SupportTickets;

using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Domain.Notifications;
using Neighborhood.Services.Domain.Newsletter;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> , IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Review> Reviews { get; }
        public DbSet<ReviewAnalysis> ReviewAnalyses { get; }
        public DbSet<Staff> Staffs { get;  }
        public DbSet<StaffPermission> StaffPermissions { get; }
        public DbSet<Dispute> Disputes { get; }

        public DbSet<Conversation> Conversations { get; }

        public DbSet<SupportTicket> SupportTickets { get; }

        public DbSet<SupportMessage> SupportMessages { get; }

        override protected void OnModelCreating(ModelBuilder Modelbuilder)
        {
            base.OnModelCreating(Modelbuilder);
            Modelbuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

       
        
        //Arwa
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        ////////////////////END OF ARWA //////////////////////





    }
}
