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
    }
}
