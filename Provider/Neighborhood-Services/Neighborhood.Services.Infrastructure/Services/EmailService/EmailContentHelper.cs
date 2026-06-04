
using Microsoft.AspNetCore.Hosting;
using Neighborhood.Services.Domain.ProblemTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Services.EmailService
{
    public class EmailContentHelper
    {
#pragma warning disable CS0618
        private readonly IWebHostEnvironment _environment;
        private readonly Dictionary<string, string> _templates;

#pragma warning disable CS0618
        public EmailContentHelper(IWebHostEnvironment environment)
        {
            _environment = environment;
            _templates = LoadTemplates();
        }
        public string BuildEmailContent(string templateName,
        string bodyContent,
        Dictionary<string, string> placeholders = null!)
        {
            var template = _templates[templateName];
            // Inject body content
            template = template.Replace("{{BodyContent}}", bodyContent);
            // Apply custom placeholders
            if (placeholders != null)
            {
                foreach (var placeholder in placeholders)
                {
                    template = template.Replace(
                    $"{{{{{placeholder.Key}}}}}",
                    placeholder.Value);
                }
            }
            // Add embedded logo
            var logoPath = Path.Combine(
            _environment.ContentRootPath, "wwwroot", "email-templates", "logo.png");
            if (File.Exists(logoPath))
            {
                var logoBytes = File.ReadAllBytes(logoPath);
                var logoBase64 = Convert.ToBase64String(logoBytes);
                template = template.Replace(
                "cid:company-logo",
                $"data:image/png;base64,{logoBase64}");
            }
            return template;
        }

        public string BuildVerificationEmailContent(string ConfirmationUrl,Dictionary<string, string> placeholders = null!)
        {
            var template = _templates["EmailVerificationTemplate"];
            template = template.Replace("{{ConfirmUrl}}", ConfirmationUrl);

            //place holders {{Optional}}
            if (placeholders != null)
            {
                foreach (var placeholder in placeholders)
                {
                    template = template.Replace(
                    $"{{{{{placeholder.Key}}}}}",
                    placeholder.Value);
                }
            }

            return template;
        }

        public string BuildPasswordResetMailContent(string ResettingUrl, Dictionary<string, string> placeholders = null!)
        {
            var template = _templates["PasswordResetTemplate"];
            template = template.Replace("{{ResettingUrl}}", ResettingUrl);

            //place holders {{Optional}}
            if (placeholders != null)
            {
                foreach (var placeholder in placeholders)
                {
                    template = template.Replace(
                    $"{{{{{placeholder.Key}}}}}",
                    placeholder.Value);
                }
            }

            return template;
        }

        public string BuildVerificationBookingMailContent(
            int BookingId,
            string TechnicianName,
            DateTime Time,
            string Problem, 
            Dictionary<string, string> placeholders = null!)
        {
            var template = _templates["BookingVerificationTemplate"];
            template = template.Replace("{{BookingId}}", BookingId.ToString());
            template = template.Replace("{{TechnicianName}}", TechnicianName);

            template = template.Replace("{{Time}}", Time.ToString());
            template = template.Replace("{{Problem}}", Problem);



            //place holders {{Optional}}
            if (placeholders != null)
            {
                foreach (var placeholder in placeholders)
                {
                    template = template.Replace(
                    $"{{{{{placeholder.Key}}}}}",
                    placeholder.Value);
                }
            }

            return template;
        }

        public string BuildNewsletterMailContent(
            string MainTitle,
            string SubTitle,
            string Content,
            Dictionary<string, string> placeholders = null!)
        {
            var template = _templates["NewsletterTemplate"];
            template = template.Replace("{{MainTitle}}", MainTitle);
            template = template.Replace("{{SubTitle}}", SubTitle);
            template = template.Replace("{{NewsContent}}", Content);


            //place holders {{Optional}}
            if (placeholders != null)
            {
                foreach (var placeholder in placeholders)
                {
                    template = template.Replace(
                    $"{{{{{placeholder.Key}}}}}",
                    placeholder.Value);
                }
            }

            return template;
        }


        private Dictionary<string, string> LoadTemplates()
        {
            var templates = new Dictionary<string, string>();
            var templatePath = Path.Combine(
            _environment.ContentRootPath, "wwwroot", "EmailTemplates");
            foreach (var file in Directory.GetFiles(templatePath, "*.html"))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                templates[name] = File.ReadAllText(file);
            }
            return templates;
        }
    }
}
