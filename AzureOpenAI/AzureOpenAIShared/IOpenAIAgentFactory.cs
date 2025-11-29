using AgentConfiguration;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.OpenAI;
using AzureOpenAIShared.Stores;
using Microsoft.Extensions.AI;
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
    /// Optionally includes tools and applies function call middleware.
    /// </summary>
    /// <param name="request">The request containing agent type and optional tools.</param>
    /// <returns>The created AIAgent.</returns>
    AIAgent CreateAdvancedAIAgent(CreateAdvancedAIAgentRequest request);

    /// <summary>
    /// Creates an AIAgent with a vector store.
    /// </summary>
    /// <param name="agentType">The type of agent to create, which determines the configuration to use.</param>
    /// <param name="vectorStoreType">The type of vector store to use for the agent.</param>
    /// <returns>The created AIAgent.</returns>
    AIAgent CreateAgentWithVectorStore(AgentType agentType, VectorStoresTypes vectorStoreType);
}

