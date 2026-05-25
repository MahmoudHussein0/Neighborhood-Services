using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ApplicationUser;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Domain.Notifications;
using Neighborhood.Services.Domain.Newsletter;
using Neighborhood.Services.Domain.Reviews;
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
