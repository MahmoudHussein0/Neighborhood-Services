using MediatR;
using Neighborhood.Services.Application.Technicians.DTOs;

namespace Neighborhood.Services.Application.Technicians.Queries
{
    public class GetTechnicianByIdQuery : IRequest<TechnicianDetailsDTO>
    {
        public int Id { get; set; }
    }
}
