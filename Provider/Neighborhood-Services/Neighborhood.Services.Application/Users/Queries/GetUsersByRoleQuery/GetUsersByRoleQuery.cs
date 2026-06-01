using MediatR;
using Neighborhood.Services.Application.Users.DTOs;
using Neighborhood.Services.Domain.ApplicationUsers;

namespace Neighborhood.Services.Application.Users.Queries
{
    public class GetUsersByRoleQuery : IRequest<List<UserSummaryDTO>>
    {
        public ApplicationUserRole ApplicationUserRole { get; set; }
    }
}
