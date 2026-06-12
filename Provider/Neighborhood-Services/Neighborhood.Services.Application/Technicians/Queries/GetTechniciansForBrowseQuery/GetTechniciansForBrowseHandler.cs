using MediatR;
using Neighborhood.Services.Application.Technicians.DTOs;
using Neighborhood.Services.Application.Technicians.Interfaces;

namespace Neighborhood.Services.Application.Technicians.Queries
{
    public class GetTechniciansForBrowseHandler(ITechnicianRepository technicianRepository)
        : IRequestHandler<GetTechniciansForBrowseQuery, List<TechnicianCardDTO>>
    {
        private readonly ITechnicianRepository _technicianRepository = technicianRepository;

        public Task<List<TechnicianCardDTO>> Handle(GetTechniciansForBrowseQuery request, CancellationToken cancellationToken)
            => _technicianRepository.GetActiveForBrowseAsync();
    }
}
