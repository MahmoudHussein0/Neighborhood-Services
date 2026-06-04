using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Neighborhood.Services.Application.Shared.Email
{
    public class EmailDto
    {
      
        public string To { get; set; } = string.Empty;

        public string ContactName { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;
    }
}
