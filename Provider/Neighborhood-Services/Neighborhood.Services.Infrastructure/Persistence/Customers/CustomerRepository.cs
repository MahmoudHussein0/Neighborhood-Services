using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.PublicProfiles.DTOs;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Customers;
using Neighborhood.Services.Domain.Reviews;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.Customers
{
    public class CustomerRepository : GenericRepository<Customer, int>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<Customer?> GetByUserIdAsync(string applicationUserId)
        {
            return await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(customer => customer.ApplicationUserId == applicationUserId && !customer.IsDeleted);
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(customer => !customer.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Customer>> GetActiveAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(customer => customer.IsActive && !customer.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Customer>> GetDeletedAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(customer => customer.IsDeleted)
                .ToListAsync();
        }

        public async Task<PublicProfileDto?> GetPublicProfileAsync(int customerId)
        {
            var profile = await (
                from customer in _context.Customers.AsNoTracking()
                where customer.Id == customerId && !customer.IsDeleted
                join user in _context.Users on customer.ApplicationUserId equals user.Id
                select new PublicProfileDto
                {
                    Role = "Customer",
                    ApplicationUserId = customer.ApplicationUserId,
                    FullName = user.FullName,
                    Photo = user.Photo,
                    Latitude = user.Location != null ? (double?)user.Location.Y : null,
                    Longitude = user.Location != null ? (double?)user.Location.X : null,
                    MemberSince = user.CreatedAt
                }
            ).FirstOrDefaultAsync();

            if (profile is null)
                return null;

            profile.CompletedJobs = await _context.Bookings.AsNoTracking()
                .CountAsync(b => b.CustomerId == customerId && b.Status == BookingStatus.Completed);

            // Approved reviews about this customer (RevieweeId == their ApplicationUserId), with reviewer name/photo.
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
            profile.AverageRating = profile.Reviews.Count > 0
                ? Math.Round((decimal)profile.Reviews.Average(r => r.Rating), 1)
                : 0m;

            return profile;
        }
    }
}
