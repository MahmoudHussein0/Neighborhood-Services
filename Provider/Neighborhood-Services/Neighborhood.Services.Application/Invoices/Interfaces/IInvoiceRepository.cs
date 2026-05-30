using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Invoices;
namespace Neighborhood.Services.Application.Invoices.Interfaces
{
    public interface IInvoiceRepository : IGenericRepository<Invoice, int>
    {
    }
}