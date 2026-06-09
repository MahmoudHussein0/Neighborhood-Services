using MediatR;
using Neighborhood.Services.Application.Technicians.DTOs;

namespace Neighborhood.Services.Application.Technicians.Queries
{
    // Customer-facing browse list for the "Find Technician" page.
    public class GetTechniciansForBrowseQuery : IRequest<List<TechnicianCardDTO>>
    {
    }
}
