using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Categories
{
    public class CategoriesConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(C => C.Name)
                   .IsRequired()     
                   .HasMaxLength(50);

            builder.Property(C => C.IsDeleted)
                          .HasDefaultValue(false);

            builder.HasQueryFilter(C => !C.IsDeleted);

            builder.Property(C => C.CreatedAt)
                         .HasDefaultValueSql("GETUTCDATE()");

            builder.HasMany(C => C.ProblemTypes)
                            .WithOne(P => P.Category)
                            .HasForeignKey(P => P.CategoryId)
                            .OnDelete( DeleteBehavior.NoAction);


            builder.HasMany(C => C.TechnicianCategories)
                   .WithOne(TC => TC.Category)
                   .HasForeignKey(TC => TC.CategoryId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
