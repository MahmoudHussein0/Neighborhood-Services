using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Invoices.DTOs;
using Neighborhood.Services.Application.Invoices.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ApplicationUsers;

namespace Neighborhood.Services.Application.Invoices.Queries.GetMyInvoices
{
    public class GetMyInvoicesQueryHandler : IRequestHandler<GetMyInvoicesQuery, IEnumerable<InvoiceResponseDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public GetMyInvoicesQueryHandler(
            IInvoiceRepository invoiceRepository, 
            UserManager<ApplicationUser> userManager, 
            ICurrentUserService currentUserService)
        {
            _invoiceRepository = invoiceRepository;
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<InvoiceResponseDto>> Handle(GetMyInvoicesQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId ?? throw new UnauthorizedException("User is not authenticated.");
            var user = await _userManager.Users
                .Include(u => u.Customer)
                .Include(u => u.Technician)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null) throw new UnauthorizedException("User not found.");

            // Materialize once to avoid multiple enumeration
            List<Neighborhood.Services.Domain.Invoices.Invoice> invoices;

            if (user.Customer != null)
            {
                invoices = (await _invoiceRepository.GetByCustomerIdAsync(user.Customer.Id)).ToList();
            }
            else if (user.Technician != null)
            {
                invoices = (await _invoiceRepository.GetByTechnicianIdAsync(user.Technician.Id)).ToList();
            }
            else
            {
                // Staff or user with no role, return empty list
                return new List<InvoiceResponseDto>();
            }

            if (!invoices.Any())
                return new List<InvoiceResponseDto>();

            var customerIds = invoices.Select(i => i.CustomerId).Distinct().ToList();
            var technicianIds = invoices.Select(i => i.TechnicianId).Distinct().ToList();

            var customers = await _userManager.Users
                .Where(u => u.Customer != null && customerIds.Contains(u.Customer.Id))
                .Select(u => new { u.Customer.Id, u.FullName })
                .ToDictionaryAsync(x => x.Id, x => x.FullName, cancellationToken);

            var technicians = await _userManager.Users
                .Where(u => u.Technician != null && technicianIds.Contains(u.Technician.Id))
                .Select(u => new { u.Technician.Id, u.FullName })
                .ToDictionaryAsync(x => x.Id, x => x.FullName, cancellationToken);

            return invoices.Select(invoice => new InvoiceResponseDto
            {
                Id = invoice.Id,
                BookingId = invoice.BookingId,
                TransactionId = invoice.TransactionId,
                CustomerId = invoice.CustomerId,
                CustomerName = customers.TryGetValue(invoice.CustomerId, out var cName) ? cName : null,
                TechnicianId = invoice.TechnicianId,
                TechnicianName = technicians.TryGetValue(invoice.TechnicianId, out var tName) ? tName : null,
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
            });
        }
    }
}
