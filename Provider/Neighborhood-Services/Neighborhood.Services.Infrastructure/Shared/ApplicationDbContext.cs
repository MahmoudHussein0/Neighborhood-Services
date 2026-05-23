using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Message;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Shared
{
    public class ApplicationDbContext:DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        //arwa
        // Arwa 
        public DbSet<Message> Messages { get; set; }
        public DbSet <Conversation> Conversations { get; set; }
    }
}
