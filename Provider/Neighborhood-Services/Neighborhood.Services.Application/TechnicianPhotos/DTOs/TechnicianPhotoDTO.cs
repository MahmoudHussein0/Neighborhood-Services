namespace Neighborhood.Services.Application.TechnicianPhotos.DTOs
{
    public class TechnicianPhotoDTO
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public int TechnicianId { get; set; }
    }
}
