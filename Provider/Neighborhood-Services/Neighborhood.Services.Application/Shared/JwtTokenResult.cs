namespace Neighborhood.Services.Application.Shared
{
    public class JwtTokenResult
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
