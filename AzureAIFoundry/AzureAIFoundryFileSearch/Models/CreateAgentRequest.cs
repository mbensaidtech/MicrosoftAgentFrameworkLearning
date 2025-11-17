using AgentConfiguration;

namespace AzureAIFoundryFileSearch.Models;

/// <summary>
/// Request model for creating a new agent.
/// </summary>
public class CreateAgentRequest
{
    /// <summary>
    /// Gets or sets the optional agent type. If not provided, defaults to ProductSearchAgent.
    /// </summary>
    public AgentType? AgentType { get; set; }
}

