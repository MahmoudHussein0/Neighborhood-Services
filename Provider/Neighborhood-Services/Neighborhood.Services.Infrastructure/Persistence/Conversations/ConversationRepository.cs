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
            var result = await _context.Conversations.Include(e=>e.Messages).FirstOrDefaultAsync(e => e.BookingId == bookingId);
            return result;
        }

        //public Task<Conversation> GetByBookingId(int bookingId)
        //{
        //    var result = await _context.Conversations.FirstOrDefaultAsync(e => e.BookingId == bookingId);
        //}

        public async Task<Conversation> GetByUserId(string userId)
        {
            var result = await (_context.Conversations.Include(e => e.Messages).FirstOrDefaultAsync(e=>e.Messages.First().SenderId == userId));
            //var result2 = await (_context.Conversations.Include(e => e.Messages).FirstOrDefaultAsync(e => e.Messages. == userId));


            return result;
        }

        public async Task<Customer> GetClient(int roomId)
        {
           
            var result= await (_context.Conversations.Include(e => e.Booking).Select(e => e.Booking.Customer)).FirstAsync();
            return result;
        }

        public async Task<Message> GetLastMessage(int roomId)
        {
            return _context.Conversations.FirstOrDefault(e => e.Id==roomId)?.lastMessage??new Message() { content="no messages in this room"};
        }

        public async Task<Technician> GetTechnician(int roomId)
        {
            var result = await (_context.Conversations.Include(e => e.Booking).Select(e => e.Booking.Technician)).FirstAsync();
            return result;
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
