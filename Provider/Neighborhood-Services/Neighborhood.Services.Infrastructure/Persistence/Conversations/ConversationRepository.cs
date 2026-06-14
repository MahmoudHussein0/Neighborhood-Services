using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Conversations;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Customers;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Infrastructure.Persistence.Conversations
{
    public class ConversationRepository: GenericRepository<Conversation, int>,IConversationRepository
    {
        public ConversationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Conversation> GetByBookingId(int bookingId)
        {
            //var result= await (_context.Conversations.Include(e => e.Booking).Where(e => e.BookingId == bookingId)).FirstOrDefaultAsync();
            var result = await _context.Conversations.Include(e=>e.Booking).FirstOrDefaultAsync(e => e.BookingId == bookingId);
            return result;
        }

        public async Task<Conversation> GetWithLastMessageSender(int bookingId)
        {
            //var result= await (_context.Conversations.Include(e => e.Booking).Where(e => e.BookingId == bookingId)).FirstOrDefaultAsync();
            var result = await _context.Conversations.Include(e => e.Messages).ThenInclude(e => e.Sender).Where(e=>e.BookingId==bookingId).FirstAsync();
            return result;
        }

        public async Task<List<Conversation>> GetWithLastMessageSenderbyUserId(string UserId)
        {
            return await _context.Conversations
        .Include(c => c.Messages)
            .ThenInclude(m => m.Sender)
            .Include(e=>e.Booking)
        .Where(c => c.Messages.Any(m => m.SenderId == UserId))
        .OrderByDescending(c => c.Messages.Max(m => m.createdAt))
        .ToListAsync();
        }
            
        
        //public Task<Conversation> GetByBookingId(int bookingId)
        //{
        //    var result = await _context.Conversations.FirstOrDefaultAsync(e => e.BookingId == bookingId);
        //}


        //عبثية
        public async Task<Conversation> GetByUserId(string userId)
        {
            var result = await (_context.Conversations.Include(e => e.Messages).FirstOrDefaultAsync(e=>e.Messages.First().SenderId == userId));
            //var result2 = await (_context.Conversations.Include(e => e.Messages).FirstOrDefaultAsync(e => e.Messages. == userId));


            return result;
        }

        public async Task<Customer> GetClient(int roomId)
        {
           
            var result= await (_context.Conversations.Include(e => e.Booking).Select(e => e.Booking.Customer)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Message> GetLastMessage(int roomId)
        {
            return _context.Conversations.Include(e=>e.Messages).FirstOrDefault(e => e.Id==roomId)?.lastMessage??new Message() { content="no messages in this room"};
        }

        public async Task<Technician> GetTechnician(int roomId)
        {
            var result = await (_context.Conversations.Include(e => e.Booking).Select(e => e.Booking.Technician)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<string?> GetAvatar(
    int conversationId,
    string currentUserId)
        {
            var message = await _context.Messages
                .Include(m => m.Sender)
                .Where(m =>
                    m.ConversationId == conversationId &&
                    m.SenderId != currentUserId)
                .FirstOrDefaultAsync();

            return message?.Sender?.Photo;
        }

        public async Task<string?> GetOther(
    int conversationId,
    string currentUserId)
        {
            var message = await _context.Messages
                .Include(m => m.Sender)
                .Where(m =>
                    m.ConversationId == conversationId &&
                    m.SenderId != currentUserId)
                .FirstOrDefaultAsync();

            return message?.Sender?.FullName;
        }

        Task<ApplicationUser> IConversationRepository.GetClient(int roomId)
        {
            throw new NotImplementedException();
        }

        Task<ApplicationUser> IConversationRepository.GetTechnician(int roomId)
        {
            throw new NotImplementedException();
        }
    }
}
