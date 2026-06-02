using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Application.Technicians.Interfaces
{
    public interface ITechnicianRepository : IGenericRepository<Technician, int>
    {
        Task CreateAsync(Technician technician);
        Task<Technician?> GetByUserIdAsync(string applicationUserId);
        Task<List<Technician>> GetAllActiveAsync();
        Task<List<Technician>> GetByVerificationStatusAsync(TechnicianVerificationStatus verificationStatus);
        Task<List<Technician>> GetAvailableAsync();
    }
}
