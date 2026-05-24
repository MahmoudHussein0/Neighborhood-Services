using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Infrastructure.Persistence.Staffs;

public class StaffRepository : IStaffRepository
{
    private readonly AppDbContext _context;

    public StaffRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Staff staff)
    {
        await _context.Staffs.AddAsync(staff);
    }

    public async Task<Staff?> GetByIdAsync(int id)
    {
        return await _context.Staffs
            .Include(s => s.Permissions)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Staff>> GetAllAsync()
    {
        return await _context.Staffs.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task SetActiveAsync(int id, bool isActive)
    {
        await _context.Staffs
            .Where(s => s.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsActive, isActive));
    }
}