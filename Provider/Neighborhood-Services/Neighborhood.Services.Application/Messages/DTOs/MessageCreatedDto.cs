using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Messages.DTOs
{
    public class MessageCreatedDto
    {
        public string scss_msg = "Message Created Successfully!";
        public string senderId;
        public string content;
    }
}
