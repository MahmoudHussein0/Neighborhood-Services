using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Services.EmailService
{
    public class EmailConfiguration
    {
        public string CompanyName { get; set; }
        public string FromAddress { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }
}
