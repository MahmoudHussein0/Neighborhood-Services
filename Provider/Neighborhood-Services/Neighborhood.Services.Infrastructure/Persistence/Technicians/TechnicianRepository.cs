using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Technicians.DTOs;
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

        public async Task<List<TechnicianCardDTO>> GetActiveForBrowseAsync()
        {
            return await (
                from technician in _context.Technicians.AsNoTracking()
                where technician.IsActive && !technician.IsDeleted
                join user in _context.Users on technician.ApplicationUserId equals user.Id
                select new TechnicianCardDTO
                {
                    Id = technician.Id,
                    ApplicationUserId = technician.ApplicationUserId,
                    FullName = user.FullName,
                    Photo = user.Photo,
                    Rating = technician.Rating,
                    Experience = technician.Experience,
                    MaxTravelDistance = technician.MaxTravelDistance,
                    VerificationStatus = technician.VerificationStatus,
                    IsAvailable = technician.IsAvailable,
                    Latitude = user.Location != null ? (double?)user.Location.Y : null,
                    Longitude = user.Location != null ? (double?)user.Location.X : null,
                    Categories = technician.TechnicianCategories.Select(tc => new TechnicianCardCategoryDTO
                    {
                        Id = tc.Category.Id,
                        NameEn = tc.Category.NameEn,
                        NameAr = tc.Category.NameAr,
                        Icon = tc.Category.Icon
                    }).ToList()
                }
            ).ToListAsync();
        }
    }
}
