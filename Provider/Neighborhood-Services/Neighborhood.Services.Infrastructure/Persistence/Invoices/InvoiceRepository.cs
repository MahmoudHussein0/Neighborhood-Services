using Neighborhood.Services.Application.Invoices.Interfaces;
using Neighborhood.Services.Domain.Invoices;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
namespace Neighborhood.Services.Infrastructure.Persistence.Invoices
{
    public class InvoiceRepository : GenericRepository<Invoice, int>, IInvoiceRepository
    {
        public InvoiceRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}