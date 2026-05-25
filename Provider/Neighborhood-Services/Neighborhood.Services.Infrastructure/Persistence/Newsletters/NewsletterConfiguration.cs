using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Domain.Newsletter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Newsletters
{
    public class NewsletterConfiguration : IEntityTypeConfiguration<Newsletter>
    {
        public void Configure(EntityTypeBuilder<Newsletter> builder)
        {
           builder.HasKey(x => x.Id);
           
           builder.Property(e=>e.email).IsRequired();

           builder.Property(e => e.subscribedAt).HasDefaultValue(DateTime.UtcNow);
        }
    }
}
