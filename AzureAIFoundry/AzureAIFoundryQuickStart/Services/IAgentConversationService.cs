using AgentConfiguration;
using Microsoft.Agents.AI;
using AzureAIFoundryQuickStart.Models;

namespace AzureAIFoundryQuickStart.Services;

/// <summary>
/// Interface for agent conversation service.
/// </summary>
public interface IAgentConversationService
{
    /// <summary>
    /// Sends a message to an agent and gets a response.
    /// </summary>
    /// <param name="agentType">The type of agent to use.</param>
    /// <param name="request">The request containing the message and optional thread ID.</param>
    /// <returns>The agent's response.</returns>
    Task<AgentRunResponse> SendMessageAsync(AgentType agentType, SendMessageRequest request);
}

