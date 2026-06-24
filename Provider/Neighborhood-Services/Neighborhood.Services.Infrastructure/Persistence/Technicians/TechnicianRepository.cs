using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.PublicProfiles.DTOs;
using Neighborhood.Services.Application.Technicians.DTOs;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Reviews;
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

        public async Task<PublicProfileDto?> GetPublicProfileAsync(int technicianId)
        {
            var profile = await (
                from technician in _context.Technicians.AsNoTracking()
                where technician.Id == technicianId && !technician.IsDeleted
                join user in _context.Users on technician.ApplicationUserId equals user.Id
                select new PublicProfileDto
                {
                    Role = "Technician",
                    ApplicationUserId = technician.ApplicationUserId,
                    FullName = user.FullName,
                    Photo = user.Photo,
                    Latitude = user.Location != null ? (double?)user.Location.Y : null,
                    Longitude = user.Location != null ? (double?)user.Location.X : null,
                    MemberSince = user.CreatedAt,
                    AverageRating = technician.Rating,
                    Experience = technician.Experience,
                    VerificationStatus = technician.VerificationStatus.ToString(),
                    Categories = technician.TechnicianCategories.Select(tc => new PublicProfileCategoryDto
                    {
                        Id = tc.Category.Id,
                        NameEn = tc.Category.NameEn,
                        NameAr = tc.Category.NameAr,
                        Icon = tc.Category.Icon
                    }).ToList()
                }
            ).FirstOrDefaultAsync();

            if (profile is null)
                return null;

            profile.CompletedJobs = await _context.Bookings.AsNoTracking()
                .CountAsync(b => b.TechnicianId == technicianId && b.Status == BookingStatus.Completed);

            await PopulateReviewsAsync(profile);
            return profile;
        }

        // Loads the approved reviews about this user (RevieweeId == their ApplicationUserId),
        // joined with the reviewer's name/photo. The headline rating comes from Technician.Rating
        // (the canonical value shown on browse/offer cards), so only the count is set here.
        private async Task PopulateReviewsAsync(PublicProfileDto profile)
        {
            profile.Reviews = await (
                from review in _context.Reviews.AsNoTracking()
                where review.RevieweeId == profile.ApplicationUserId && review.Status == ReviewStatus.Approved
                join reviewer in _context.Users on review.ReviewerId equals reviewer.Id into rj
                from reviewer in rj.DefaultIfEmpty()
                orderby review.CreatedAt descending
                select new PublicReviewDto
                {
                    Id = review.Id,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt,
                    ReviewerName = reviewer != null ? reviewer.FullName : string.Empty,
                    ReviewerPhoto = reviewer != null ? reviewer.Photo : string.Empty
                }
            ).ToListAsync();

            profile.ReviewCount = profile.Reviews.Count;
        }

        public async Task<Dictionary<int, string>> GetNamesByIdsAsync(IReadOnlyCollection<int> technicianIds)
        {
            if (technicianIds is null || technicianIds.Count == 0)
                return new Dictionary<int, string>();

            return await (
                from technician in _context.Technicians.AsNoTracking()
                where technicianIds.Contains(technician.Id)
                join user in _context.Users on technician.ApplicationUserId equals user.Id
                select new { technician.Id, user.FullName }
            ).ToDictionaryAsync(x => x.Id, x => x.FullName);
        }

        public async Task<List<int>> GetCategoryIdsAsync(int technicianId)
        {
            return await _context.TechnicianCategories
                .AsNoTracking()
                .Where(tc => tc.TechnicianId == technicianId)
                .Select(tc => tc.CategoryId)
                .ToListAsync();
        }
    }
}
