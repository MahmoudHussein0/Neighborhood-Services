using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Escrows;
namespace Neighborhood.Services.Application.Escrows.Interfaces
{
    public interface IEscrowRepository : IGenericRepository<Escrow, int>
    {
    }
}