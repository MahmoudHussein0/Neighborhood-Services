using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Invoices;
namespace Neighborhood.Services.Application.Invoices.Interfaces
{
    public interface IInvoiceRepository : IGenericRepository<Invoice, int>
    {
        Task<Invoice?> GetByBookingIdAsync(int bookingId);
        Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Invoice>> GetByTechnicianIdAsync(int technicianId);
        Task VoidAsync(int invoiceId);
    }
}