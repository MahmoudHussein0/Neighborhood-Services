using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.ProblemTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.ProblemTypes
{
    internal class ProblemTypesConfiguration : IEntityTypeConfiguration<ProblemType>
    {
        public void Configure(EntityTypeBuilder<ProblemType> builder)
        {

            builder.Property(PT => PT.Name)
                   .IsRequired()
                   .HasMaxLength(50);


            builder.Property(PT => PT.MinPrice)
                    .IsRequired()
                    .HasColumnType("DECIMAL(18,2)");


            builder.Property(PT => PT.MaxPrice)
                 .IsRequired()
                 .HasColumnType("DECIMAL(18,2)");

            builder.Property(PT => PT.IsDeleted)
                   .HasDefaultValue(false);


            //builder.HasQueryFilter(PT => !PT.IsDeleted);

            builder.Property(PT => PT.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");



            builder.HasMany(PT => PT.HistoricalPricing)
                   .WithOne(HP => HP.ProblemType)
                   .HasForeignKey(HP => HP.ProblemTypeId)
                   .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
