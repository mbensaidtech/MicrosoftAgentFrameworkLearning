using AzureAIFoundryShared.Models;
using AzureAIFoundryStart.Models;

namespace AzureAIFoundryStart.Services;

/// <summary>
/// Interface for thread management service.
/// </summary>
public interface IThreadService
{
    /// <summary>
    /// Sends a message to an agent and gets a response. The agent type is read from request.Context.AgentType.
    /// </summary>
    /// <param name="request">The request containing the message, optional thread ID, and agent type in context.</param>
    /// <returns>The agent's response.</returns>
    Task<AgentResponse> SendMessageAsync(SendMessageRequest request);
}

