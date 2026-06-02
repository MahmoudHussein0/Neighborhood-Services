using MediatR;
using Neighborhood.Services.Application.Escrows.DTOs;
namespace Neighborhood.Services.Application.Escrows.Commands.RefundEscrow
{
    public class RefundEscrowCommand : IRequest<EscrowResponseDto>
    {
        public int EscrowId { get; set; }
    }
}