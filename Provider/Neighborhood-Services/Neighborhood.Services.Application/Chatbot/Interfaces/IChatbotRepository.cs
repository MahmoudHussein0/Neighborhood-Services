using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Chatbot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Chatbot.Interfaces
{
    public interface IChatbotRepository : IGenericRepository<ChatbotSession,int>
    {
        Task<ChatbotSession?> GetSessionWithMessagesAsync(int sessionId);

        // All sessions for a user, newest first (no messages — just for listing)
        Task<List<ChatbotSession>> GetUserSessionsAsync(string userId);
    }
}
