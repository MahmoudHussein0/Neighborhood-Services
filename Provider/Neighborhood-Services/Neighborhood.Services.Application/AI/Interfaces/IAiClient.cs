using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Neighborhood.Services.Application.AI.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AI.Interfaces
{
    public interface IAiClient
    {
        Task<string> CompleteAsync(string systemprompt, string userprompt, string? imageUrl = null, AiCallContext? log=null);
        Task<string> ChatAsync(ChatHistory history, string systemPromp, AiCallContext? log=null);

        // Like ChatAsync, but the model may CALL the supplied tools (Semantic Kernel plugin
        // objects) on its own — function calling is auto-invoked. Each object's [KernelFunction]
        // methods become callable. Used to make the chatbot a real tool-using agent.
        Task<string> ChatWithToolsAsync(ChatHistory history, string systemPrompt, IEnumerable<object> tools, AiCallContext? log = null);
    }
}
