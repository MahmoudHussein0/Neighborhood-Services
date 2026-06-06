using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Invoices.Commands.VoidInvoice;
using Neighborhood.Services.Application.Invoices.Queries.GetInvoiceByBookingId;
using Neighborhood.Services.Application.Invoices.Queries.GetInvoiceByCustomerId;
using Neighborhood.Services.Application.Invoices.Queries.GetInvoicesByTechnicianId;
using Neighborhood.Services.Application.Invoices.Services;
using Neighborhood.Services.Application.Invoices.Commands.CreateInvoice;

namespace Neighborhood.Services.API.Invoices
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IInvoicePdfService _invoicePdfService;

        public InvoicesController(IMediator mediator, IInvoicePdfService invoicePdfService)
        {
            _mediator = mediator;
            _invoicePdfService = invoicePdfService;
        }

        [HttpGet("booking/{bookingId:int}")]
        public async Task<IActionResult> GetByBookingId(int bookingId)
        {
            var result = await _mediator.Send(new GetInvoiceByBookingIdQuery { BookingId = bookingId });
            return Ok(result);
        }

        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var result = await _mediator.Send(new GetInvoicesByCustomerIdQuery { CustomerId = customerId });
            return Ok(result);
        }

        [HttpGet("technician/{technicianId:int}")]
        public async Task<IActionResult> GetByTechnicianId(int technicianId)
        {
            var result = await _mediator.Send(new GetInvoicesByTechnicianIdQuery { TechnicianId = technicianId });
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{invoiceId:int}/void")]
        public async Task<IActionResult> VoidInvoice(int invoiceId)
        {
            var result = await _mediator.Send(new VoidInvoiceCommand { InvoiceId = invoiceId });
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("booking/{bookingId:int}/pdf")]
        public async Task<IActionResult> GetPdfByBooking(int bookingId)
        {
            var invoice = await _mediator.Send(new GetInvoiceByBookingIdQuery { BookingId = bookingId });
            var pdfBytes = _invoicePdfService.GenerateInvoicePdf(invoice);
            return File(pdfBytes, "application/pdf", $"invoice-{invoice.Id}.pdf");
        }
    }
}
