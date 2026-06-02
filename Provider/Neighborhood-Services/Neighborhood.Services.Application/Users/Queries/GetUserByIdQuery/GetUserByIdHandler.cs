using MediatR;
using Microsoft.AspNetCore.Identity;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Users.DTOs;
using Neighborhood.Services.Application.Users.Interfaces;
using Neighborhood.Services.Domain.ApplicationUsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Users.Queries.GetUserByIdQuery
{
    public class GetUserByIdHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, UserDetailsDTO>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<UserDetailsDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            var userDetails = new UserDetailsDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Age = user.Age,
                Photo = user.Photo,
                IsActive = user.IsActive,
                ApplicationUserRole = user.ApplicationUserRole
            };
            return userDetails;
        }
    }
}
