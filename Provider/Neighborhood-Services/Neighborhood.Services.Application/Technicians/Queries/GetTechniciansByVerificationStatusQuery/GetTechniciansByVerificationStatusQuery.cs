using MediatR;
using Neighborhood.Services.Application.Technicians.DTOs;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Application.Technicians.Queries
{
    public class GetTechniciansByVerificationStatusQuery : IRequest<List<TechnicianSummaryDTO>>
    {
        public TechnicianVerificationStatus VerificationStatus { get; set; }
    }
}
