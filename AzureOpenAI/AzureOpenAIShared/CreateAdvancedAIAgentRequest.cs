using AgentConfiguration;
using Microsoft.Extensions.AI;

namespace AzureOpenAIShared;

/// <summary>
/// Request object for creating an advanced AI agent.
/// </summary>
public class CreateAdvancedAIAgentRequest
{
    /// <summary>
    /// Gets or sets the type of agent to create, which determines the configuration to use.
    /// </summary>
    public AgentType AgentType { get; set; }

    /// <summary>
    /// Gets or sets the optional list of AI tools to make available to the agent.
    /// </summary>
    public List<AITool>? Tools { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to apply function call middleware for logging tool calls.
    /// Default is false. When true, function call details will be logged when tools are invoked.
    /// </summary>
    public bool EnableFunctionCallMiddleware { get; set; } = false;
}

