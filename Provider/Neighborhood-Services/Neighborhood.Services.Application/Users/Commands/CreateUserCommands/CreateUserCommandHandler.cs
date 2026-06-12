using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Application.Users.Interfaces;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Customers;
using Neighborhood.Services.Domain.Staffs;
using Neighborhood.Services.Domain.Technicians;
using NetTopologySuite.Geometries;

namespace Neighborhood.Services.Application.Users.Commands.CreateUserCommands
{
    internal class CreateUserCommandHandler(
        IUserRepository userRepository,
        ICustomerRepository customerRepository,
        ITechnicianRepository technicianRepository,
        IStaffRepository staffRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<CreateUserCommand, string>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICustomerRepository _customerRepository = customerRepository;
        private readonly ITechnicianRepository _technicianRepository = technicianRepository;
        private readonly IStaffRepository _staffRepository = staffRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Email,
                Photo = request.Photo,
                Age = request.Age,
                ApplicationUserRole = request.ApplicationUserRole,
                Location = new Point(request.Longitude, request.Latitude) { SRID = 4326 },
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RefferalCode = Guid.NewGuid().ToString()[..8].ToUpper()
            };

            var result = await _userRepository.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await CreateRoleProfileAsync(user.Id, request, cancellationToken);
            return user.Id;
        }

        private async Task CreateRoleProfileAsync(
            string applicationUserId,
            CreateUserCommand request,
            CancellationToken cancellationToken)
        {
            switch (request.ApplicationUserRole)
            {
                case ApplicationUserRole.Customer:
                    if (await _customerRepository.GetByUserIdAsync(applicationUserId) is null)
                    {
                        await _customerRepository.CreateAsync(new Customer
                        {
                            ApplicationUserId = applicationUserId,
                            IsDeleted = false,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                    break;

                case ApplicationUserRole.Technician:
                    if (await _technicianRepository.GetByUserIdAsync(applicationUserId) is null)
                    {
                        await _technicianRepository.CreateAsync(new Technician
                        {
                            ApplicationUserId = applicationUserId,
                            NationalId = request.NationalId?.Trim() ?? string.Empty,
                            Experience = request.Experience?.Trim() ?? string.Empty,
                            MaxTravelDistance = request.MaxTravelDistance ?? 0,
                            Rating = 0,
                            VerificationStatus = TechnicianVerificationStatus.Pending,
                            IsAvailable = false,
                            IsDeleted = false,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                    break;

                case ApplicationUserRole.Staff:
                    if (!await _staffRepository.ExistsByUserIdAsync(applicationUserId, cancellationToken))
                    {
                        await _staffRepository.AddAsync(new Staff
                        {
                            UserId = applicationUserId,
                            Role = StaffRole.TechnicalSupport,
                            IsActive = false,
                            CreatedAt = DateTime.UtcNow,
                            Permissions = new List<StaffPermission>()
                        });
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                    break;
            }
        }
    }
}
