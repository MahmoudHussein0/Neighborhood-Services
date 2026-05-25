using Microsoft.EntityFrameworkCore;
//using Neighborhood.Services.Application.Modules.Conversations;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Conversations
{
    public class ConversationRepository: GenericRepository<Conversation, int>
    {
        public ConversationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Conversation> GetByUserId(string userId)
        {
            return _context.Conversations.FirstOrDefault(e => e.Messages.First().SenderId.ToString() == userId) ;
        }

        public async Task<Message> GetLastMessage(int roomId)
        {
            return _context.Conversations.FirstOrDefault(e => e.Id==roomId)?.lastMessage??new Message() { content="no messages in this room"};
        }


    }
}
