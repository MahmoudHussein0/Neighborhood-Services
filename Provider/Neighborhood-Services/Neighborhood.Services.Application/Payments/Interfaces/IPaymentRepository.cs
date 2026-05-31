using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Payments;
namespace Neighborhood.Services.Application.Payments.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<PaymentMethod, int>
    {
        Task<IEnumerable<PaymentMethod>> GetByUserIdAsync(string userId);
    }
}
