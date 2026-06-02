using MediatR;
using Neighborhood.Services.Application.Users.DTOs;
using Neighborhood.Services.Domain.ApplicationUsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Users.Queries.GetUserByIdQuery
{
    public class GetUserByIdQuery : IRequest<UserDetailsDTO>
    {
        public string Id { get; set; } = string.Empty;
    }
}
