using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using AzureOpenAIAgentWithThreads.Models;
using AzureOpenAIShared;
using AzureOpenAIShared.Stores;
using Microsoft.Agents.AI;
using static CommonUtilities.ColoredConsole;
using System.Text.Json;

namespace AzureOpenAIAgentWithThreads.Services;

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
        _agent = openAIAgentFactory.CreateAgentWithVectorStore(AgentType.GlobalAgent, VectorStoresTypes.InMemory);
    }

    /// <summary>
    /// Sends a message to the agent and gets a response.
    /// </summary>
    /// <param name="message">The message to send to the agent.</param>
    /// <param name="threadId">Optional thread ID to continue an existing conversation.</param>
    /// <returns>The agent's response.</returns>
    public async Task<AgentResponse> SendMessageAsync(string message, string? threadId = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be null or empty.", nameof(message));
        }

        AgentThread thread;
        
        if (string.IsNullOrWhiteSpace(threadId))
        {
            thread = _agent.GetNewThread();
        }
        else
        {
            var agentThreadState = new AgentThreadState { StoreState = threadId };
            var threadStateElement = JsonSerializer.SerializeToElement(agentThreadState);
            thread = _agent.DeserializeThread(threadStateElement);
        }
        var agentRunResponse = await _agent.RunAsync(message, thread);
        agentRunResponse.LogTokenUsage();
        AgentResponse response = agentRunResponse.ToAgentResponse(thread);
        return response;
    }
}
