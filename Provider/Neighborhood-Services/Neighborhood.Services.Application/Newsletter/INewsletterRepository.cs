using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Domain.Newsletter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Newsletter
{
    public interface INewsletterRepository : IGenericRepository<Domain.Newsletter.Newsletter, int>
    {
       // public int id { set; get; }
    }
}

