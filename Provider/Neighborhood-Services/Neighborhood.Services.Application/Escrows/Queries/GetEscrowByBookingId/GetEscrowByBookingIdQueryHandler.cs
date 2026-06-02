using MediatR;
using Neighborhood.Services.Application.Escrows.DTOs;
using Neighborhood.Services.Application.Escrows.Interfaces;
namespace Neighborhood.Services.Application.Escrows.Queries.GetEscrowByBookingId
{
    public class GetEscrowByBookingIdQueryHandler : IRequestHandler<GetEscrowByBookingIdQuery, EscrowResponseDto>
    {
        private readonly IEscrowRepository _escrowRepository;

        public GetEscrowByBookingIdQueryHandler(IEscrowRepository escrowRepository)
        {
            _escrowRepository = escrowRepository;
        }

        public async Task<EscrowResponseDto> Handle(GetEscrowByBookingIdQuery request, CancellationToken cancellationToken)
        {
            var escrow = await _escrowRepository.GetByBookingIdAsync(request.BookingId)
                ?? throw new KeyNotFoundException($"Escrow with Booking ID {request.BookingId} not found.");

            return new EscrowResponseDto
            {
                Id = escrow.Id,
                BookingId = escrow.BookingId,
                WalletId = escrow.WalletId,
                Amount = escrow.Amount,
                Status = escrow.Status,
                HeldAt = escrow.HeldAt,
                ReleasedAt = escrow.ReleasedAt
            };
        }
    }
}