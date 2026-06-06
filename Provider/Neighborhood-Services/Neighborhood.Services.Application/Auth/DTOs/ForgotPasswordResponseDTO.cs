namespace Neighborhood.Services.Application.Auth.DTOs
{
    public class ForgotPasswordResponseDTO
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
    }
}
