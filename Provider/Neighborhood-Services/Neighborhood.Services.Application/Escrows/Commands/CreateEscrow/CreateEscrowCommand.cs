using MediatR;
using Neighborhood.Services.Application.Escrows.DTOs;
namespace Neighborhood.Services.Application.Escrows.Commands.CreateEscrow
{
    public class CreateEscrowCommand : IRequest<EscrowResponseDto>
    {
        public int BookingId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
    }
}