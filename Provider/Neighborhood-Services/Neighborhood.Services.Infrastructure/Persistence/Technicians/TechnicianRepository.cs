using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.Technicians;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using Neighborhood.Services.Application.Technicians.DTOs;


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

       
        //arwa
        public async Task<List<ComprehensiveTechDTO>> GetWithUserDetails()
        {
            var q1= await _context.Technicians
                .Join(_context.Users, t => t.ApplicationUserId, u => u.Id,
               
                (t, u) => new ComprehensiveTechDTO
                {
                    technicianId=t.Id,
                    fullName=u.FullName,
                    technicianUserId=u.Id

                   
              }).ToListAsync();

            return q1;
               
        }

        public async Task<ComprehensiveTechDTO?> GetWithUserDetailsById( int techId)
        {
            var q1 = await _context.Technicians.Where(e => e.Id == techId)
                .Join(_context.Users, t => t.ApplicationUserId, u => u.Id,

                (t, u) => new ComprehensiveTechDTO
                {
                    technicianId = t.Id,
                    fullName = u.FullName,
                    technicianUserId = u.Id


                }).FirstOrDefaultAsync();

            return q1;

        }
        //end of arwa
    }
}
