using Neighborhood.Services.Application.PublicProfiles.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.DTOs;
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


        // Customer-facing browse projection (joins ApplicationUser for name/photo/location + categories).
        Task<List<TechnicianCardDTO>> GetActiveForBrowseAsync();

        // Public profile (details + stats + approved reviews with reviewer name/photo) for a technician.
        Task<PublicProfileDto?> GetPublicProfileAsync(int technicianId);

        // Maps technician ids -> display name (ApplicationUser.FullName). Used to label bookings lists.
        Task<Dictionary<int, string>> GetNamesByIdsAsync(IReadOnlyCollection<int> technicianIds);
    }
}
