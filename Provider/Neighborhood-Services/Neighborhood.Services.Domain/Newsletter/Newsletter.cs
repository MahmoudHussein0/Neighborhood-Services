using Neighborhood.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Neighborhood.Services.Domain.Newsletter
{
    public class Newsletter:BaseEntity<int>
    {
        //updatteeee

        [EmailAddress]
        public string email {  get; set; }
        public DateTime subscribedAt { get; set; }
    }
}
