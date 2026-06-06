using MediatR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Neighborhood.Services.Application.AgentLogs.Commands;
using Neighborhood.Services.Application.AI.DTOs;
using Neighborhood.Services.Application.AI.Interfaces;

namespace Neighborhood.Services.Infrastructure.Services.AI
{
    public class SemanticKernelClient : IAiClient
    {
        private readonly Kernel _kernel;
        private readonly IMediator _mediator;
        public SemanticKernelClient(Kernel Kernel, IMediator mediator)
        {
            _kernel = Kernel;
            _mediator = mediator;
        }

        public async Task<string> CompleteAsync(string systemPrompt, string userPrompt, string? imageUrl = null, AiCallContext? log = null)
        {
            // 1- get the ai service from the kernel
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();

            // 2. Build the conversation: system sets the rules, user sends the request
            var history = new ChatHistory();
            history.AddSystemMessage(systemPrompt);

            // checking the image
            if (imageUrl == null)
            {
                history.AddUserMessage(userPrompt);
            }
            else
            {
                var collection = new ChatMessageContentItemCollection();
                collection.Add(new TextContent(userPrompt));
                collection.Add(new ImageContent(new Uri(imageUrl)));

                history.AddUserMessage(collection);
            }
            // 3. Call the AI
            var result = await chatService.GetChatMessageContentAsync(history);
            // calc tokens 
            int? tokensUsed = null;
            if (result.Metadata?.TryGetValue("Usage", out var usage) == true && usage is OpenAI.Chat.ChatTokenUsage tokenUsage)
                tokensUsed = tokenUsage.TotalTokenCount;
            // logging 
            if (log != null)
            {
                try { 
                await _mediator.Send(new CreateAgentLogCommand
                {
                    AgentType = log.AgentType,
                    Action = log.Action,
                    Input = userPrompt,
                    Output = result.Content ?? "",
                    ReferenceType = log.ReferenceType,
                    ReferenceId = log.ReferenceId,
                    TokensUsed = tokensUsed
                });
                }
                catch
                {

                }
            }

            // 4. Return the text
            return result.Content ?? string.Empty;

        }

        public async Task<string> ChatAsync(ChatHistory history, string systemPrompt, AiCallContext? log = null)
        {
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            history.Insert(0, new ChatMessageContent(AuthorRole.System, systemPrompt));

            var result = await chatService.GetChatMessageContentAsync(history);

            if (log != null)
            {
                int? tokensUsed = null;
                if (result.Metadata?.TryGetValue("Usage", out var usage) == true && usage is OpenAI.Chat.ChatTokenUsage tokenUsage)
                    tokensUsed = tokenUsage.TotalTokenCount;

                try
                {
                    await _mediator.Send(new CreateAgentLogCommand
                    {
                        AgentType = log.AgentType,
                        Action = log.Action,
                        Input = history.LastOrDefault(m => m.Role == AuthorRole.User)?.Content ?? "",
                        Output = result.Content ?? "",
                        ReferenceType = log.ReferenceType,
                        ReferenceId = log.ReferenceId,
                        TokensUsed = tokensUsed
                    });
                }
                catch { }
            }

            return result.Content ?? string.Empty;
        }


    }
}
