using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using AzureOpenAIAgentWithFunctionTools.Models;
using AzureOpenAIAgentWithFunctionTools.Tools;
using AzureOpenAIShared;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using static CommonUtilities.ColoredConsole;
using System.Text.Json;

namespace AzureOpenAIAgentWithFunctionTools.Services;

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

        var customerTools = new CustomerTools();
        var tools = customerTools.CreateAIToolsFromInstance();

        var request = new CreateAdvancedAIAgentRequest
        {
            AgentType = AgentType.GlobalAgent,
            Tools = tools,
            EnableFunctionCallMiddleware = true
        };
        _agent = openAIAgentFactory.CreateAdvancedAIAgent(request);
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

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonResponse = JsonSerializer.Serialize(agentRunResponse, jsonOptions);
        WritePrimaryLogLine("AgentRunResponse Content:");
        WriteAssistantLine(jsonResponse);
      
        AgentResponse response = agentRunResponse.ToAgentResponse();
        return response;
    }
}

