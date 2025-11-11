using Microsoft.Agents.AI;
using Azure.AI.Agents.Persistent;
using Azure;

namespace AzureAIFoundryShared;

/// <summary>
/// Interface for agent administration service.
/// </summary>
public interface IAgentAdministration
{
    /// <summary>
    /// Creates an agent with the specified parameters.
    /// </summary>
    /// <param name="request">The request containing agent creation parameters.</param>
    /// <returns>The created agent.</returns>
    Task<AIAgent> CreateAgentAsync(CreateAgentRequest request);

    /// <summary>
    /// Gets an agent by its ID.
    /// </summary>
    /// <param name="agentId">The ID of the agent to retrieve.</param>
    /// <returns>The agent with the specified ID.</returns>
    Task<AIAgent> GetAgentByIdAsync(string agentId);

    /// <summary>
    /// Gets an agent by its ID, or creates a new agent if the ID is not provided.
    /// </summary>
    /// <param name="agentId">The ID of the agent to retrieve. If null, a new agent will be created.</param>
    /// <param name="agentType">The type of agent to create. Required when agentId is null.</param>
    /// <returns>The agent.</returns>
    Task<AIAgent> GetOrCreateAgentAsync(string? agentId, AgentConfiguration.AgentType? agentType);

    /// <summary>
    /// Creates an agent request from the specified agent type.
    /// </summary>
    /// <param name="agentType">The type of agent to create a request for.</param>
    /// <returns>The agent creation request.</returns>
    CreateAgentRequest CreateAgentRequest(AgentConfiguration.AgentType agentType);

    /// <summary>
    /// Deletes an agent by its ID.
    /// </summary>
    /// <param name="agentId">The ID of the agent to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAgentByIdAsync(string agentId);

    /// <summary>
    /// Gets a thread by its ID.
    /// </summary>
    /// <param name="threadId">The ID of the thread to retrieve.</param>
    /// <returns>The thread with the specified ID.</returns>
    Task<Response<PersistentAgentThread>> GetThreadByIdAsync(string threadId);

    /// <summary>
    /// Creates a thread.
    /// </summary>
    /// <returns>The created thread.</returns>
    Task<PersistentAgentThread> CreateThreadAsync();

    /// <summary>
    /// Gets an AgentThread by its ID for use with RunAsync.
    /// </summary>
    /// <param name="threadId">The ID of the thread to retrieve.</param>
    /// <returns>The AgentThread with the specified ID.</returns>
    Task<AgentThread> GetAIThreadAsync(string threadId);

    /// <summary>
    /// Gets a PersistentAgentThread by its ID.
    /// </summary>
    /// <param name="threadId">The ID of the thread to retrieve.</param>
    /// <returns>The PersistentAgentThread with the specified ID.</returns>
    Task<PersistentAgentThread> GetPersistentAgentThreadAsync(string threadId);
}

