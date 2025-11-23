using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using AzureOpenAIAgentStart.Models;
using AzureOpenAIShared;
using Microsoft.Agents.AI;

namespace AzureOpenAIAgentStart.Services;

/// <summary>
/// Service for agent interactions.
/// </summary>
public class AgentService : IAgentService
{
    private readonly AIAgent _agent;
    private readonly AgentConfig _agentConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentService"/> class.
    /// </summary>
    /// <param name="openAIAgentFactory">The OpenAI agent factory.</param>
    /// <param name="agentConfig">The agent configuration.</param>
    public AgentService(IOpenAIAgentFactory openAIAgentFactory, AgentConfig agentConfig)
    {
        if (openAIAgentFactory == null)
        {
            throw new ArgumentNullException(nameof(openAIAgentFactory));
        }

        if (agentConfig == null)
        {
            throw new ArgumentNullException(nameof(agentConfig));
        }

        _agentConfig = agentConfig;
        _agent = openAIAgentFactory.CreateAdvancedAIAgent(AgentType.GlobalAgent);
    }

    /// <summary>
    /// Sends a message to the agent and gets a response.
    /// </summary>
    /// <param name="message">The message to send to the agent.</param>
    /// <returns>The agent's response.</returns>
    public async Task<AgentResponse> SendMessageAsync(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be null or empty.", nameof(message));
        }

        
        var agentRunResponse = await _agent.RunAsync(message);
        agentRunResponse.LogTokenUsage();
        AgentResponse response = agentRunResponse.ToAgentResponse();
        return response;
    }
}
