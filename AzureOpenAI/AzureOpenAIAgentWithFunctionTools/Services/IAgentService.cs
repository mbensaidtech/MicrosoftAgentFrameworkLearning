using AzureOpenAIAgentWithFunctionTools.Models;

namespace AzureOpenAIAgentWithFunctionTools.Services;

/// <summary>
/// Interface for agent service.
/// </summary>
public interface IAgentService
{
    /// <summary>
    /// Sends a message to the agent and gets a response.
    /// </summary>
    /// <param name="message">The message to send to the agent.</param>
    /// <returns>The agent's response.</returns>
    Task<AgentResponse> SendMessageAsync(string message);
}

