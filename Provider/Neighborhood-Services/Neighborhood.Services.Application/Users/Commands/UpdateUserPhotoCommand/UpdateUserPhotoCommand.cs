using MediatR;

namespace Neighborhood.Services.Application.Users.Commands
{
    public class UpdateUserPhotoCommand : IRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
    }
}
