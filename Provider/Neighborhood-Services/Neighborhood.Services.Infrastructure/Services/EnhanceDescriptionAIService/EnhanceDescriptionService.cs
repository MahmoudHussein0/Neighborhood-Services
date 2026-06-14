using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Chat;

namespace Neighborhood.Services.Infrastructure.Services.EnhanceDescriptionAIService
{
    public class EnhanceDescriptionAIService
    {
        private readonly Kernel _kernel;
        ChatHistory _chatHistory;
        //ChatMessage _mssg;
        
        public EnhanceDescriptionAIService(Kernel kernel)
        {
            _kernel = kernel;
            _chatHistory= new ChatHistory();
            
        }

        public async Task <string> GenerateSuggestions(string description)
        {
          

            SystemChatMessage system_message = new SystemChatMessage("Ask the user 3 important questions to clarify his request for technician");
            UserChatMessage user_description = new UserChatMessage(description);
            _chatHistory.AddUserMessage(description);
            _chatHistory.AddSystemMessage("Ask the user 3 important questions to clarify his request for technician");
           // _mssg = new ChatMessage(system_message);

            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            var result = chatService.GetStreamingChatMessageContentsAsync(_chatHistory, kernel: _kernel);
            string response = " ";
            await foreach (var ch in result)
            {
                response += ch.Content;
            }
            _chatHistory.AddAssistantMessage(response);
            return response;
        }

    }
}
