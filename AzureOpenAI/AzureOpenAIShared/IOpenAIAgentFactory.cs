using AgentConfiguration;
using Microsoft.Agents.AI;

namespace AzureOpenAIShared;

/// <summary>
/// Interface for creating OpenAI agents.
/// </summary>
public interface IOpenAIAgentFactory
{
    /// <summary>
    /// Creates a basic AIAgent with instructions and name based on the specified agent type.
    /// </summary>
    /// <param name="agentType">The type of agent to create, which determines the configuration to use.</param>
    /// <returns>The created AIAgent.</returns>
    AIAgent CreateBasicAIAgent(AgentType agentType);

    /// <summary>
    /// Creates an advanced AIAgent with instructions and name based on the specified agent type.
    /// </summary>
    /// <param name="agentType">The type of agent to create, which determines the configuration to use.</param>
    /// <returns>The created AIAgent.</returns>
    AIAgent CreateAdvancedAIAgent(AgentType agentType);
}

