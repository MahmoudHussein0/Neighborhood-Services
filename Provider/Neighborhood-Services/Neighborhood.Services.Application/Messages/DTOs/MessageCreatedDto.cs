using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Messages.DTOs
{
    public class MessageCreatedDto

    {
        public int id {  get; set; }
        public string scss_msg = "Message Created Successfully!";
        public string senderId { set; get; }
        public string content { set; get; }

        public int BookingId { set; get; }

        public bool? hasImage { set; get; }

        public string? imageUrl { set; get; }
    }
}
