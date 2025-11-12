using Microsoft.Agents.AI;

namespace AzureAIFoundryShared;

/// <summary>
/// Interface for agent administration service.
/// </summary>
public interface IAgentAdministration
{
    /// <summary>
    /// Gets an agent by its ID, or creates a new agent if the ID is not provided.
    /// </summary>
    /// <param name="agentId">The ID of the agent to retrieve. If null, a new agent will be created.</param>
    /// <param name="agentType">The type of agent to create. Required when agentId is null.</param>
    /// <returns>The agent.</returns>
    Task<AIAgent> GetOrCreateAgentAsync(string? agentId, AgentConfiguration.AgentType? agentType);
}

