using MediatR;
using Neighborhood.Services.Domain.ApplicationUsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Users.Commands.CreateUserCommands
{
    internal class CreateUserCommand : IRequest<String>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Age { get; set; } 
        public ApplicationUserRole ApplicationUserRole { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
