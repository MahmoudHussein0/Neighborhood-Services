using Neighborhood.Services.Application.Shared;

using Neighborhood.Services.Infrastructure.Persistence.Context; 

using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Shared
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
