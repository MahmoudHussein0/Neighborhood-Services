using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Application.Staffs.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;

    public StaffService(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

  

    public async Task<List<StaffDto>> GetAllAsync()
    {
        var staffs = await _staffRepository.GetAllAsync();

        return staffs.Select(s => new StaffDto
        {
            Id = s.Id,
            UserId = s.UserId,
            Role = s.Role.ToString(),
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt
        }).ToList();
    }

    public async Task<StaffDto?> GetByIdAsync(int id)
    {
        var staff = await _staffRepository.GetByIdAsync(id);

        if (staff is null)
            return null;

        return new StaffDto
        {
            Id = staff.Id,
            UserId = staff.UserId,
            Role = staff.Role.ToString(),
            IsActive = staff.IsActive,
            CreatedAt = staff.CreatedAt
        };
    }

   

   
    public async Task CreateAsync(CreateStaffDto dto)
    {
        var staff = new Staff(
            dto.UserId,
            (StaffRole)dto.Role,
            isActive: true,              // default
            dto.CreatedByStaffId,
            createdAt: DateTime.UtcNow   // default
        );

        await _staffRepository.AddAsync(staff);
        await _staffRepository.SaveChangesAsync();
    }

    public async Task ActivateAsync(int id)
    {
        var staff = await _staffRepository.GetByIdAsync(id);

        if (staff is null)
            throw new Exception("Staff not found");

        await _staffRepository.SetActiveAsync(id, true);
    }

    public async Task DeactivateAsync(int id)
    {
        var staff = await _staffRepository.GetByIdAsync(id);

        if (staff is null)
            throw new Exception("Staff not found");

        await _staffRepository.SetActiveAsync(id, false);
    }
}