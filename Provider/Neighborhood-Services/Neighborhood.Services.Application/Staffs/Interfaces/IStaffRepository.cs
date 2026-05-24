

using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Application.Staffs.Interfaces;

public interface IStaffRepository
{
    Task AddAsync(Staff staff);
    Task<List<Staff>> GetAllAsync();
    Task<Staff?> GetByIdAsync(int id);
    Task SetActiveAsync(int id, bool isActive);  // handles both
    Task SaveChangesAsync();
}