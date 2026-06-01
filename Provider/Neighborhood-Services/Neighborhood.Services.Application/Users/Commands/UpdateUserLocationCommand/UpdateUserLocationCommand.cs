using MediatR;

namespace Neighborhood.Services.Application.Users.Commands
{
    public class UpdateUserLocationCommand : IRequest
    {
        public string Id { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
