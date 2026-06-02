using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Messages.DTOs
{
    public class MessageUpdatedDto
    {
        public int id { set; get; }
        public string content { set; get; } = string.Empty;

        public bool Read { set; get; }

        public bool Deleted { set; get; }

    }
}
