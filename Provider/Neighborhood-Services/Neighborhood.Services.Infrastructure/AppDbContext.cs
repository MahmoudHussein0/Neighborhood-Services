using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Domain.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure
{
    public class AppDbContext:DbContext

    {
        public DbSet<Review> Reviews { get; set; }

        public DbSet<ReviewAnalysis> ReviewAnalyses { get; set; }

      
    }
}
