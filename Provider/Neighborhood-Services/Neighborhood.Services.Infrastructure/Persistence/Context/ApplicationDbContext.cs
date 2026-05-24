using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ApplicationUser;
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
    }
}
