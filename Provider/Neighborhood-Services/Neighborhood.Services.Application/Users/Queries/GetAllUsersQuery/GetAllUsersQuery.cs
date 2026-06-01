using MediatR;
using Neighborhood.Services.Application.Users.DTOs;

namespace Neighborhood.Services.Application.Users.Queries
{
    public class GetAllUsersQuery : IRequest<List<UserSummaryDTO>>
    {
    }
}
