using Neighborhood.Services.Application.Messages;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;
//using Neighborhood.Services.Application.Modules.Messages;
using Neighborhood.Services.Domain.Message;



namespace Neighborhood.Services.Infrastructure.Persistence.Messages
{
    public class MessageRepository:GenericRepository<Message,int>,IMessageRepository
    {
        public MessageRepository(ApplicationDbContext context) : base(context){}

             public async Task<Message> GetByUserId(string userId)
        {
            return _context.Messages.FirstOrDefault(e => e.SenderId.ToString() == userId);
        }

        public async Task<Message> GetBConversationId(string convId)
        {
            return _context.Messages.FirstOrDefault(e => e.ConversationId.ToString() == convId);
        }

        public Task<List<Message>> GetByConversationIdAsync(string conversationId, int skip, int limit)
        {
            throw new NotImplementedException();
        }
    }

    }
