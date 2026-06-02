using Neighborhood.Services.Application.Messages;
using Neighborhood.Services.Application.Newsletter;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Domain.Newsletter;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Newsletters
{
    public class NewsletterRepository : GenericRepository<Newsletter, int>, INewsletterRepository
    {
        public NewsletterRepository(ApplicationDbContext context) : base(context)
        {
        }

    }
}
