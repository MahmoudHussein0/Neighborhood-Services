using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Shared.Email
{
    public class EmailMessageDto
    {
        public IEnumerable<MailboxAddress>? ToGroup { get; set; }
        public IEnumerable<MailboxAddress> ToTry { get; set; }

        public MailboxAddress? To { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string HtmlContent { get; set; }
        public IEnumerable<EmailAttachmentDto>? Attachments { get; set; }



        //Constructor
        //public EmailMessageDto(
        //IEnumerable<string> recipients,
        //string subject,
        //string htmlContent,
        //IEnumerable<EmailAttachmentDto> attachments = null)
        //{
        //    To = recipients.Select(r => new MailboxAddress("", r));
        //    Subject = subject;
        //    HtmlContent = htmlContent;
        //    Attachments = attachments ?? Enumerable.Empty<EmailAttachmentDto>();
        //}
    }
}
