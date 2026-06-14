using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Newsletter;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Newsletter;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.Newsletters
{
    public class NewsletterRepository : GenericRepository<Newsletter, int>, INewsletterRepository
    {
        public NewsletterRepository(ApplicationDbContext context) : base(context)
        {
        }

    
 
            public async Task<List<string>> GetEmails()
        {
            return await _context.Newsletters
                .Select(e => e.email)
                .ToListAsync();
        }
    }
    }

    

