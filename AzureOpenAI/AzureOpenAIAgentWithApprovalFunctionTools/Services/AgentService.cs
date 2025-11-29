using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using AzureOpenAIAgentWithApprovalFunctionTools.Models;
using AzureOpenAIAgentWithApprovalFunctionTools.Tools;
using AzureOpenAIShared;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using static CommonUtilities.ColoredConsole;
using System.Text.Json;

namespace AzureOpenAIAgentWithApprovalFunctionTools.Services;

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

        AgentThread thread = _agent.GetNewThread();

        var agentRunResponse = await _agent.RunAsync(message, thread);
        agentRunResponse.LogTokenUsage();

        var functionApprovalRequests = agentRunResponse.Messages
        .SelectMany(x => x.Contents)
        .OfType<FunctionApprovalRequestContent>()
        .ToList();

        foreach (var functionApprovalRequest in functionApprovalRequests)
        {
            WriteSystemLine($"We require approval to execute '{functionApprovalRequest.FunctionCall.Name}'");
            var approvalMessage = new ChatMessage(ChatRole.User, [functionApprovalRequest.CreateResponse(true)]);
            await _agent.RunAsync(approvalMessage, thread);
            agentRunResponse = await _agent.RunAsync(message, thread);
            agentRunResponse.LogTokenUsage();

        }
      
        AgentResponse response = agentRunResponse.ToAgentResponse();
        return response;
    }
}

