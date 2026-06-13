using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Technicians;
using Neighborhood.Services.Application.Technicians.DTOs;

namespace Neighborhood.Services.Application.Technicians.Interfaces
{
    public interface ITechnicianRepository : IGenericRepository<Technician, int>
    {
        Task CreateAsync(Technician technician);
        Task<Technician?> GetByUserIdAsync(string applicationUserId);
        Task<List<Technician>> GetAllActiveAsync();
        Task<List<Technician>> GetByVerificationStatusAsync(TechnicianVerificationStatus verificationStatus);
        Task<List<Technician>> GetAvailableAsync();

        
        //arwa
        Task<List<ComprehensiveTechDTO>> GetWithUserDetails();
        Task<ComprehensiveTechDTO?> GetWithUserDetailsById(int techId);
        //end of arwa


    }
}
