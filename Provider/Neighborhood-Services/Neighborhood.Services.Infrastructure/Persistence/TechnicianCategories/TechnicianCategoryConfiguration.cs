using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.TechnicionCategories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.TechnicianCategories
{
    public class TechnicianCategoryConfiguration : IEntityTypeConfiguration<TechnicianCategory>
    {
        public void Configure(EntityTypeBuilder<TechnicianCategory> builder)
        {

            builder.HasOne(TC => TC.Technician)
                   .WithMany()
                   .HasForeignKey(TC => TC.TechnicianId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(TC => TC.Category)
                   .WithMany(C => C.TechnicianCategories)
                   .HasForeignKey(TC => TC.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(TC => new { TC.TechnicianId, TC.CategoryId })
                    .IsUnique();
        }
    }
}
