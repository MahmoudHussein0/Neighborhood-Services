using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Invoices.DTOs;
using Neighborhood.Services.Application.Invoices.Interfaces;
using Neighborhood.Services.Domain.ApplicationUsers;

namespace Neighborhood.Services.Application.Invoices.Queries.GetInvoiceByBookingId
{
    public class GetInvoiceByBookingIdQueryHandler : IRequestHandler<GetInvoiceByBookingIdQuery, InvoiceResponseDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetInvoiceByBookingIdQueryHandler(IInvoiceRepository invoiceRepository, UserManager<ApplicationUser> userManager)
        {
            _invoiceRepository = invoiceRepository;
            _userManager = userManager;
        }

        public async Task<InvoiceResponseDto> Handle(GetInvoiceByBookingIdQuery request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetByBookingIdAsync(request.BookingId)
                ?? throw new KeyNotFoundException($"Invoice for booking ID {request.BookingId} not found.");

            var customerUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Customer.Id == invoice.CustomerId, cancellationToken);
            var technicianUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Technician.Id == invoice.TechnicianId, cancellationToken);

            return new InvoiceResponseDto
            {
                Id = invoice.Id,
                BookingId = invoice.BookingId,
                TransactionId = invoice.TransactionId,
                CustomerId = invoice.CustomerId,
                CustomerName = customerUser?.FullName,
                TechnicianId = invoice.TechnicianId,
                TechnicianName = technicianUser?.FullName,
                Amount = invoice.Amount,
                Tax = invoice.Tax,
                TotalAmount = invoice.TotalAmount,
                BaseAmount = invoice.BaseAmount,
                DiscountAmount = invoice.DiscountAmount,
                PromoCodeApplied = invoice.PromoCodeApplied,
                Status = invoice.Status,
                IssuedAt = invoice.IssuedAt,
                PaidAt = invoice.PaidAt,
                VoidedAt = invoice.VoidedAt
            };
        }
    }
}