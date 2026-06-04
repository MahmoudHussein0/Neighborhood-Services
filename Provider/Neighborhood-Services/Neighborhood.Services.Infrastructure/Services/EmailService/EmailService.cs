using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Neighborhood.Services.Application.Shared.Email;
using Neighborhood.Services.Domain.ProblemTypes;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Services.EmailService
{
    
    public class EmailService:IEmailService
    {
        private readonly EmailConfiguration _config;
        private readonly EmailContentHelper _contentHelper;
        private readonly IWebHostEnvironment _environment;

        public EmailService(IOptions<EmailConfiguration> config,
       // EmailContentHelper contentHelper,
        IWebHostEnvironment _environment

        )
        {
            _config = config.Value;
            _contentHelper = new EmailContentHelper(_environment);
        }
        

        public async Task SendEmailAsync(EmailMessageDto message)
        {

            using var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_config.CompanyName, _config.FromAddress));
            //email.To.AddRange(message.ToGroup?? (Enumerable.Empty<MailboxAddress>()).Append(message.To));
            email.To.AddRange(message.ToGroup ?? new List<MailboxAddress>() { message?.To ?? new MailboxAddress("","") });
            email.Subject = message.Subject;
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message.HtmlContent
            };

            // Add attachments
            if (message.Attachments != null)
            {
                foreach (var attachment in message.Attachments)
                {
                    bodyBuilder.Attachments.Add(
                    attachment.FileName,
                    attachment.Content,
                    ContentType.Parse(attachment.MimeType));
                }
            }

            email.Body = bodyBuilder.ToMessageBody();
            using var client = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                await client.ConnectAsync(_config.SmtpServer, _config.Port,
        _config.EnableSsl);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_config.Username, _config.Password);
                await client.SendAsync(email);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }

        }
    

    public async Task <EmailSendingResult>SendEmailVerificationEmailAsync(string EmailReceiver,
        string ConfirmationURL,string title="Confirm your email", IEnumerable<EmailAttachmentDto>? emailAttachments=null!)
        {
            string genratedHTML = _contentHelper.BuildVerificationEmailContent(ConfirmationURL,null!);
            EmailMessageDto message = new EmailMessageDto()
            {
                To = MailboxAddress.Parse(EmailReceiver),
                HtmlContent = genratedHTML,
                Subject=title,
                Attachments=emailAttachments


            };

            using var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_config.CompanyName, _config.FromAddress));
            email.To.Add(message.To);
            email.Subject = message.Subject;
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message.HtmlContent
            };

            // Add attachments
            if (message.Attachments != null)
            {
                foreach (var attachment in message.Attachments)
                {
                    bodyBuilder.Attachments.Add(
                    attachment.FileName,
                    attachment.Content,
                    ContentType.Parse(attachment.MimeType));
                }
            }
            email.Body = bodyBuilder.ToMessageBody();
            using var client = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                await client.ConnectAsync(_config.SmtpServer, _config.Port,
        _config.EnableSsl);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_config.Username, _config.Password);
                await client.SendAsync(email);
                return new EmailSendingResult()
                {
                    To = EmailReceiver
                };
            }
            finally
            {
                await client.DisconnectAsync(true);
            }

        }

        public async Task<EmailSendingResult> SendPasswordResetEmailAsync(string EmailReceiver,
       string PasswordResetURL, string title = "Reset Your Password", IEnumerable<EmailAttachmentDto>? emailAttachments = null!)
        {
            string genratedHTML = _contentHelper.BuildPasswordResetMailContent(PasswordResetURL, null!);
            EmailMessageDto message = new EmailMessageDto()
            {
                To = MailboxAddress.Parse(EmailReceiver),
                HtmlContent = genratedHTML,
                Subject = title,
                Attachments = emailAttachments


            };

            using var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_config.CompanyName, _config.FromAddress));
            email.To.Add(message.To);
            email.Subject = message.Subject;
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message.HtmlContent
            };

            // Add attachments
            if (message.Attachments != null)
            {
                foreach (var attachment in message.Attachments)
                {
                    bodyBuilder.Attachments.Add(
                    attachment.FileName,
                    attachment.Content,
                    ContentType.Parse(attachment.MimeType));
                }
            }
            email.Body = bodyBuilder.ToMessageBody();
            using var client = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                await client.ConnectAsync(_config.SmtpServer, _config.Port,
        _config.EnableSsl);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_config.Username, _config.Password);
                await client.SendAsync(email);
                return new EmailSendingResult()
                {
                    To = EmailReceiver
                };
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
            
        }

        public async Task<EmailSendingResult> SendBookingVerificationEmainAsync(
            string EmailReceiver,
     int BookingId,
     DateTime Time,
     string Problem, 
     string TechnicianName, 
     string title = "Booking is Verified", 
     IEnumerable<EmailAttachmentDto>? emailAttachments = null!)
        {
            string genratedHTML = _contentHelper.BuildVerificationBookingMailContent(BookingId, TechnicianName, Time, Problem, null);
            EmailMessageDto message = new EmailMessageDto()
            {
                To = new MailboxAddress("", EmailReceiver),
                HtmlContent = genratedHTML,
                Subject = title,
                Attachments = emailAttachments


            };

            using var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_config.CompanyName, _config.FromAddress));
            email.To.Add(message.To);
            email.Subject = message.Subject;
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message.HtmlContent
            };

            // Add attachments
            if (message.Attachments != null)
            {
                foreach (var attachment in message.Attachments)
                {
                    bodyBuilder.Attachments.Add(
                    attachment.FileName,
                    attachment.Content,
                    ContentType.Parse(attachment.MimeType));
                }
            }
            email.Body = bodyBuilder.ToMessageBody();
            using var client = new MailKit.Net.Smtp.SmtpClient();

            if (string.IsNullOrWhiteSpace(_config.SmtpServer))
                throw new InvalidOperationException("EmailHost configuration is missing");
            if (_config.Port<0)
                throw new InvalidOperationException("Invalid Port configuration");
            if (string.IsNullOrWhiteSpace(_config.Username))
                throw new InvalidOperationException("EmailUsername configuration is missing");
            if (string.IsNullOrWhiteSpace(_config.Password))
                throw new InvalidOperationException("EmailPassword configuration is missing");


            try
            {
               await client.ConnectAsync(_config.SmtpServer, _config.Port,_config.EnableSsl);
                //client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_config.Username, _config.Password);
                await client.SendAsync(email);
                return new EmailSendingResult()
                {

                    To = EmailReceiver,

                    content = message.HtmlContent,



                };

            }
        
            
            finally
            {
                await client.DisconnectAsync(true);
                
               
            }
           

        }

        //Sending Newsletter Emails
    }

}

