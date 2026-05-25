using Neighborhood.Services.Application.Payments.Interfaces;
using Neighborhood.Services.Domain.Payments;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
namespace Neighborhood.Services.Infrastructure.Persistence.Payments
{
    public class PaymentRepository : GenericRepository<PaymentMethod, int>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}