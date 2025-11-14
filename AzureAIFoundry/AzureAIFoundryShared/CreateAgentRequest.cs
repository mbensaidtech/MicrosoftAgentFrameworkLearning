using Azure.AI.Agents.Persistent;
namespace AzureAIFoundryShared;

/// <summary>
/// Request object for creating an agent.
/// </summary>
public class CreateAgentRequest
{
    /// <summary>
    /// Gets or sets the deployment name to use for the agent.
    /// </summary>
    public string DeploymentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the agent.
    /// </summary>
    public string AgentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the instructions for the agent.
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tool resources for the agent.
    /// </summary>
    public ToolResources? ToolResources { get; set; } = null;

    /// <summary>
    /// Gets or sets the tools for the agent.
    /// </summary>
    public List<ToolDefinition>? Tools { get; set; } = null;
}

