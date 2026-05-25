using Neighborhood.Services.Application.Escrows.Interfaces;
using Neighborhood.Services.Domain.Escrows;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
namespace Neighborhood.Services.Infrastructure.Persistence.Escrows
{
    public class EscrowRepository : GenericRepository<Escrow, int>, IEscrowRepository
    {
        public EscrowRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}