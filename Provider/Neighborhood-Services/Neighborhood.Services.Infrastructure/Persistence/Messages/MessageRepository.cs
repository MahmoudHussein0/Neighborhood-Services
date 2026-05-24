using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Application.Modules.Messages;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.Messages
{
    public class MessageRepository:GenericRepository<Message,int>
    {
        public MessageRepository(ApplicationDbContext context) : base(context)
        {

        }

    }
}
