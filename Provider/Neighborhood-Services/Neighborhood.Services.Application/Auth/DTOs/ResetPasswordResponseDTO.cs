namespace Neighborhood.Services.Application.Auth.DTOs
{
    public class ResetPasswordResponseDTO
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }
}
