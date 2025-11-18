using System.Text.Json.Serialization;

namespace AzureAIFoundrySO.Models;

/// <summary>
/// Information about an agent.
/// </summary>
public class AgentInfo
{
    /// <summary>
    /// Gets or sets the agent ID.
    /// </summary>
    [JsonPropertyName("agentId")]
    public string AgentId { get; set; } = string.Empty;
}

