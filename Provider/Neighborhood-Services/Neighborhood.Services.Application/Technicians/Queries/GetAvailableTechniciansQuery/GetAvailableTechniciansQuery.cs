using MediatR;
using Neighborhood.Services.Application.Technicians.DTOs;

namespace Neighborhood.Services.Application.Technicians.Queries
{
    public class GetAvailableTechniciansQuery : IRequest<List<TechnicianSummaryDTO>>
    {
    }
}
