using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Messages.DTOs
{
    public class MessageSelectedDto
    {
        public int ConversationId { set; get; }
        public int MessageId { set; get;  }
        public string Content { set; get; }
        public bool Read {  set; get; }
        public bool Deleted { set; get; }

    }
}
