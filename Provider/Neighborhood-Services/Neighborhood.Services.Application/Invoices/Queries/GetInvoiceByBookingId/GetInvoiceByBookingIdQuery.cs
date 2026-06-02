using MediatR;
using Neighborhood.Services.Application.Invoices.DTOs;
namespace Neighborhood.Services.Application.Invoices.Queries.GetInvoiceByBookingId
{
    public class GetInvoiceByBookingIdQuery : IRequest<InvoiceResponseDto>
    {
        public int BookingId { get; set; }
    }
}