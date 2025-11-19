using AzureAIFoundrySO.Models;

namespace AzureAIFoundrySO.Services;

/// <summary>
/// Interface for thread management service.
/// </summary>
public interface IThreadService
{
    /// <summary>
    /// Sends a message to an agent and gets a response. The agent type is read from request.Context.AgentType.
    /// </summary>
    /// <typeparam name="T">The type of structured output expected from the agent.</typeparam>
    /// <param name="request">The request containing the message, optional thread ID, and agent type in context.</param>
    /// <returns>The agent's response with structured output.</returns>
    Task<StructuredAgentResponse<T>> SendMessageAsync<T>(SendMessageRequest request);
}

