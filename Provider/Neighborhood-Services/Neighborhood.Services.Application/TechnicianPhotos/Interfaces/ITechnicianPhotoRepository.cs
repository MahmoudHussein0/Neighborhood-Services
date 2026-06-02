using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.TechnicianPhotos;

namespace Neighborhood.Services.Application.TechnicianPhotos.Interfaces
{
    public interface ITechnicianPhotoRepository : IGenericRepository<TechnicianPhoto, int>
    {
        Task CreateAsync(TechnicianPhoto technicianPhoto);
        Task<List<TechnicianPhoto>> GetAllPhotosAsync();
        Task<List<TechnicianPhoto>> GetByTechnicianIdAsync(int technicianId);
        Task<List<TechnicianPhoto>> GetByUserIdAsync(string applicationUserId);
        Task DeletePhotoAsync(TechnicianPhoto technicianPhoto);
    }
}
