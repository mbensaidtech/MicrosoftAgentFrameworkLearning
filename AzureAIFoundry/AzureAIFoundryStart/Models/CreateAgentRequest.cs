using AgentConfiguration;

namespace AzureAIFoundryStart.Models;

/// <summary>
/// Request model for creating a new agent.
/// </summary>
public class CreateAgentRequest
{
    /// <summary>
    /// Gets or sets the optional agent type. If not provided, defaults to GlobalAgent.
    /// </summary>
    public AgentType? AgentType { get; set; }
}

