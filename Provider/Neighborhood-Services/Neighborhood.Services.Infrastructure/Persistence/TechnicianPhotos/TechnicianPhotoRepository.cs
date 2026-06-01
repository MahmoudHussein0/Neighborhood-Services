using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.TechnicianPhotos.Interfaces;
using Neighborhood.Services.Domain.TechnicianPhotos;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.TechnicianPhotos
{
    public class TechnicianPhotoRepository : GenericRepository<TechnicianPhoto, int>, ITechnicianPhotoRepository
    {
        public TechnicianPhotoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(TechnicianPhoto technicianPhoto)
        {
            await _context.TechnicianPhotos.AddAsync(technicianPhoto);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(TechnicianPhoto technicianPhoto)
        {
            _context.TechnicianPhotos.Update(technicianPhoto);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TechnicianPhoto>> GetAllPhotosAsync()
        {
            return await _context.TechnicianPhotos
                .AsNoTracking()
                .OrderByDescending(technicianPhoto => technicianPhoto.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<TechnicianPhoto>> GetByTechnicianIdAsync(int technicianId)
        {
            return await _context.TechnicianPhotos
                .AsNoTracking()
                .Where(technicianPhoto => technicianPhoto.TechnicianId == technicianId)
                .OrderByDescending(technicianPhoto => technicianPhoto.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<TechnicianPhoto>> GetByUserIdAsync(string applicationUserId)
        {
            return await _context.TechnicianPhotos
                .AsNoTracking()
                .Where(technicianPhoto => technicianPhoto.ApplicationUserId == applicationUserId)
                .OrderByDescending(technicianPhoto => technicianPhoto.CreatedAt)
                .ToListAsync();
        }

        public async Task DeletePhotoAsync(TechnicianPhoto technicianPhoto)
        {
            _context.TechnicianPhotos.Remove(technicianPhoto);
            await _context.SaveChangesAsync();
        }
    }
}
