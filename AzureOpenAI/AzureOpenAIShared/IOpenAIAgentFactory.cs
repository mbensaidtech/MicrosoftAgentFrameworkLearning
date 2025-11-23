using AgentConfiguration;
using Microsoft.Agents.AI;
using AzureOpenAIShared.Stores;

namespace AzureOpenAIShared;

/// <summary>
/// Interface for creating OpenAI agents.
/// </summary>
public interface IOpenAIAgentFactory
{
    /// <summary>
    /// Creates a basic AIAgent.
    /// </summary>
    /// <returns>The created AIAgent.</returns>
    AIAgent CreateBasicAIAgent();

    /// <summary>
    /// Creates an advanced AIAgent with instructions and name based on the specified agent type.
    /// </summary>
    /// <param name="agentType">The type of agent to create, which determines the configuration to use.</param>
    /// <returns>The created AIAgent.</returns>
    AIAgent CreateAdvancedAIAgent(AgentType agentType);

    /// <summary>
    /// Creates an AIAgent with a vector store.
    /// </summary>
    /// <param name="agentType">The type of agent to create, which determines the configuration to use.</param>
    /// <param name="vectorStoreType">The type of vector store to use for the agent.</param>
    /// <returns>The created AIAgent.</returns>
    AIAgent CreateAgentWithVectorStore(AgentType agentType, VectorStoresTypes vectorStoreType);
}

