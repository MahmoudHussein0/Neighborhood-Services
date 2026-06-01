using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.Technicians;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.Technicians
{
    public class TechnicianRepository : GenericRepository<Technician, int>, ITechnicianRepository
    {
        public TechnicianRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(Technician technician)
        {
            await _context.Technicians.AddAsync(technician);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(Technician technician)
        {
            _context.Technicians.Update(technician);
            await _context.SaveChangesAsync();
        }

        public async Task<Technician?> GetByUserIdAsync(string applicationUserId)
        {
            return await _context.Technicians
                .AsNoTracking()
                .FirstOrDefaultAsync(technician => technician.ApplicationUserId == applicationUserId && !technician.IsDeleted);
        }

        public async Task<List<Technician>> GetAllActiveAsync()
        {
            return await _context.Technicians
                .AsNoTracking()
                .Where(technician => !technician.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Technician>> GetByVerificationStatusAsync(TechnicianVerificationStatus verificationStatus)
        {
            return await _context.Technicians
                .AsNoTracking()
                .Where(technician => technician.VerificationStatus == verificationStatus && !technician.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Technician>> GetAvailableAsync()
        {
            return await _context.Technicians
                .AsNoTracking()
                .Where(technician => technician.IsAvailable && technician.IsActive && !technician.IsDeleted)
                .ToListAsync();
        }
    }
}
