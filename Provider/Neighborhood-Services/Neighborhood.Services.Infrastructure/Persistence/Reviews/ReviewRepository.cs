using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Domain.Reviews;

namespace Neighborhood.Services.Infrastructure.Persistence.Reviews;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        return await _context.Reviews.FindAsync(id);
    }

    public async Task<List<Review>> GetAllAsync()
    {
        return await _context.Reviews.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        await _context.Reviews
       .Where(r => r.Id == id)
       .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsDeleted, true));
    }
}