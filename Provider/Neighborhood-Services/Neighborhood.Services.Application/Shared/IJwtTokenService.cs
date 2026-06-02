using Neighborhood.Services.Domain.ApplicationUsers;

namespace Neighborhood.Services.Application.Shared
{
    public interface IJwtTokenService
    {
        JwtTokenResult GenerateToken(ApplicationUser user);
    }
}
