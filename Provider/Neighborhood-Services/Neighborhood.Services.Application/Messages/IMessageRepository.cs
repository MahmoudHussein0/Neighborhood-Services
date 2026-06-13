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
        Task<List<Message>> GetByConversationIdAsync(int conversationId);

        Task<List<Message>> GetByConversationAndUserIdAsync(int conversationId,string userId);

        public Task<List<Message>> GetByBookingIdAsync(int BookingId);

    }
}
