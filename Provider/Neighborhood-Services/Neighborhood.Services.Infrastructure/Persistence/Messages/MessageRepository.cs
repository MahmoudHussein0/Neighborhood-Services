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
using Microsoft.EntityFrameworkCore;



namespace Neighborhood.Services.Infrastructure.Persistence.Messages
{
    public class MessageRepository:GenericRepository<Message,int>,IMessageRepository
    {
        public MessageRepository(ApplicationDbContext context) : base(context){}

             public async Task<Message?> GetByUserId(string userId)
        {
            var res=await _context.Messages.FirstOrDefaultAsync(e => e.SenderId == userId && !e.IsDeleted);
            return res;
        }

        //public async Task<Message> GetBConversationId(string convId)
        //{
        //    return _context.Messages.FirstOrDefault(e => e.ConversationId.ToString() == convId);
        //}

        public async Task<List<Message>> GetByConversationIdAsync(int conversationId)
        {
            var res = await _context.Messages.Include(e=>e.Sender).Where(e=>e.ConversationId==conversationId && !e.IsDeleted).ToListAsync();
            return res;
        }

        public async Task<List<Message>> GetByConversationAndUserIdAsync(int conversationId, string userId)
        {
           var res= await _context.Messages
                .Where(e=>e.SenderId==userId&& e.ConversationId == conversationId&& !e.IsDeleted)
                .ToListAsync();
            return res;
        }

        public async Task<List<Message>> GetByBookingIdAsync(int BookingId)
        {
            var res = await _context.Messages.Include(e=>e.Sender).Include(e=>e.Conversation).ThenInclude(e=>e.Booking)
                .Where(e=>e.Conversation.BookingId==BookingId).ToListAsync();
            return res;
        }
    }
    }

    
