using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Newsletter;

namespace Neighborhood.Services.Application.Newsletter
{
    public interface INewsletterRepository : IGenericRepository<Neighborhood.Services.Domain.Newsletter.Newsletter, int>
    {
        public  Task<List<string>> GetEmails();
    }
}
