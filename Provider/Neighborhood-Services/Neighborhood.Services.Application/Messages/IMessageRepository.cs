using Neighborhood.Services.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Application.Shared;


namespace Neighborhood.Services.Application.Messages
{
    public interface IMessageRepository:IGenericRepository<Message,int>
    {
        Task<List<Message>> GetByConversationIdAsync(string conversationId, int skip, int limit);

    }
}
