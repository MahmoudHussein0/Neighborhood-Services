using MediatR;

namespace Neighborhood.Services.Application.Users.Commands
{
    public class UpdateUserProfileCommand : IRequest
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
