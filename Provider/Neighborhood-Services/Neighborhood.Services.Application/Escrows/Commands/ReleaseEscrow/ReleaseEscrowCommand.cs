using MediatR;
using Neighborhood.Services.Application.Escrows.DTOs;
namespace Neighborhood.Services.Application.Escrows.Commands.ReleaseEscrow
{
    public class ReleaseEscrowCommand : IRequest<EscrowResponseDto>
    {
        public int EscrowId { get; set; }
    }
}