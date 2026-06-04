using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Shared.Email
{
    public class EmailAttachmentDto
    {
        public string FileName { get; }
        public byte[] Content { get; }
        public string MimeType { get; }
        
        //Constructor
        public EmailAttachmentDto(string fileName, byte[] content, string mimeType)
        {
            FileName = fileName;
            Content = content;
            MimeType = mimeType;
        }
    }
}
