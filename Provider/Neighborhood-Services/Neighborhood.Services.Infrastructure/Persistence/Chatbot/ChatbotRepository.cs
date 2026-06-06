using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Chatbot.Interfaces;
using Neighborhood.Services.Domain.Chatbot;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Chatbot
{
    public class ChatbotRepository:GenericRepository<ChatbotSession,int>,IChatbotRepository
    {

        public ChatbotRepository(ApplicationDbContext context):base(context)
        {
            
        }
        public async Task<ChatbotSession?> GetSessionWithMessagesAsync(int sessionId)
        {
            return await _context.ChatbotSessions
                .Include(s => s.Messages.OrderBy(m => m.CreatedAt))
                .FirstOrDefaultAsync(s => s.Id == sessionId && !s.IsDeleted);
        }

        public async Task<List<ChatbotSession>> GetUserSessionsAsync(string userId)
        {
            return await _context.ChatbotSessions
                .Where(s => s.UserId == userId && !s.IsDeleted)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }
    }
}
