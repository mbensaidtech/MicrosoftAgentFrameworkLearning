using AgentConfiguration;
using AzureAIFoundryStart.Models;

namespace AzureAIFoundryStart.Services;

/// <summary>
/// Interface for agent management service.
/// </summary>
public interface IAgentService
{
    /// <summary>
    /// Creates a new agent.
    /// </summary>
    /// <param name="agentType">The type of agent to create.</param>
    /// <returns>Information about the created agent.</returns>
    Task<AgentInfo> CreateAgentAsync(AgentType agentType);
}

