using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Conversations
{
    public interface IConversationRepository : IGenericRepository<Conversation, int>
    {
        public Task<Conversation> GetByUserId(string userId);
        public Task<Conversation> GetByBookingId(int bookingId);
        public Task<Message> GetLastMessage(int roomId);

        public Task<ApplicationUser> GetTechnician(int roomId);
        public Task<ApplicationUser> GetClient(int roomId);

        public Task<Conversation> GetWithLastMessageSender(int bookingId);

        public Task<List<Conversation>> GetWithLastMessageSenderbyUserId(string UserId);

        public Task<string?> GetAvatar( int conversationId,string currentUserId);

        public Task<string?> GetOther( int conversationId,string currentUserId);
    }
        
}
