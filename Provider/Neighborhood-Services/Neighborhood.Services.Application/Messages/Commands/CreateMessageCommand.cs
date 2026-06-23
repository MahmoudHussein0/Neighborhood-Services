
using MediatR;
using Neighborhood.Services.Application.Messages.DTOs;

namespace Neighborhood.Services.Application.Messages.Commands
{
    public class CreateMessageCommand: IRequest<MessageCreatedDto>
    {
        //foriegn key
      //  public int ConversationId { get; set; }

        public int BookingId { get; set; }
        //foriegn Key
        public string? SenderId { get; set; }
        public string content { get; set; }
        public bool isRead { get; set; } = false;

        public DateTime createdAt = DateTime.UtcNow;

        public bool? hasImage { set; get; }=false;
        public string? imageUrl { set; get; }

    }
}
//احنا نكريت المسدج بالبوكنج أيدي ونشوف لو البوكنج ده له كونفرزيشن نضيف لها المسدج لو ملهوش يبقى 
//نكريت كونفرزيشن جديدة أساسا
