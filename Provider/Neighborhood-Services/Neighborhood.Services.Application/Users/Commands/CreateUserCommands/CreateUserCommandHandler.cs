using MediatR;
using Microsoft.AspNetCore.Identity;
using Neighborhood.Services.Application.Users.Interfaces;
using Neighborhood.Services.Domain.ApplicationUsers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Users.Commands.CreateUserCommands
{
    internal class CreateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<CreateUserCommand, string>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                 FullName = request.FullName,
                 Email = request.Email,
                 UserName = request.Email,
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
            return user.Id;
        }
    }
}
