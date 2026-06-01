using MediatR;
using Neighborhood.Services.Application.Technicians.DTOs;

namespace Neighborhood.Services.Application.Technicians.Queries
{
    public class GetTechnicianByUserIdQuery : IRequest<TechnicianDetailsDTO>
    {
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
