using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.favorites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Favorites
{
    public class FavoritesConfiguration : IEntityTypeConfiguration<Favorite>
    {

        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(f => new
            { f.CustomerId, f.TechnicianId })
                .IsUnique();

            builder
                .HasOne(e => e.Customer)
                .WithMany(e => e.Favorites)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(e => e.Technician).WithMany(e => e.Favorites)
                .HasForeignKey(e => e.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }

    }

