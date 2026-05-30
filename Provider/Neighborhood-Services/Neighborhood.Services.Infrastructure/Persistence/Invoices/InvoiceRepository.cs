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

        public async Task<Invoice?> GetByBookingIdAsync(int bookingId)
        {
            var res = await GetByConditionAsync(i => i.BookingId == bookingId);
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId)
        {
            return await GetByConditionAsync(i => i.CustomerId == customerId);
        }

        public async Task<IEnumerable<Invoice>> GetByTechnicianIdAsync(int technicianId)
        {
            return await GetByConditionAsync(i => i.TechnicianId == technicianId);
        }

        public async Task VoidAsync(int invoiceId)
        {
            var invoice = await GetByIdAsync(invoiceId);
            if (invoice is null) return;

            invoice.Status = InvoiceStatus.Voided;
            invoice.VoidedAt = DateTime.UtcNow;
            await UpdateAsync(invoice);
        }
    }
}