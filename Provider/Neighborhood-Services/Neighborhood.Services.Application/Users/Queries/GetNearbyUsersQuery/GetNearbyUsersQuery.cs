using MediatR;
using Neighborhood.Services.Application.Users.DTOs;

namespace Neighborhood.Services.Application.Users.Queries
{
    public class GetNearbyUsersQuery : IRequest<List<UserSummaryDTO>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double DistanceInMeters { get; set; }
    }
}
