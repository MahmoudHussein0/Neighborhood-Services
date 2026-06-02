using MediatR;
using Neighborhood.Services.Application.Escrows.DTOs;
namespace Neighborhood.Services.Application.Escrows.Queries.GetEscrowByBookingId
{
    public class GetEscrowByBookingIdQuery : IRequest<EscrowResponseDto>
    {
        public int BookingId { get; set; }
    }
}