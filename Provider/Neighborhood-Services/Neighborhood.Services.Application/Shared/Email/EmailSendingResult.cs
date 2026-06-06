using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Shared.Email
{
    public class EmailSendingResult
    {
       public  string message = "Email Sent Succefully";
        public string To { set; get; }
        public string content { set; get; }
    }
}
