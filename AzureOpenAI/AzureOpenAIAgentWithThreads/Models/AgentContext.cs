using AgentConfiguration;
using System.Text.Json.Serialization;

namespace AzureOpenAIAgentWithThreads.Models;

/// <summary>
/// Context object for conversation continuity.
/// </summary>
public class AgentContext
{
    /// <summary>
    /// Gets or sets the thread ID for conversation continuity (optional).
    /// </summary>
    public string? ThreadId { get; set; }

    /// <summary>
    /// Gets or sets the agent ID to use (optional).
    /// </summary>
    public string? AgentId { get; set; }

    /// <summary>
    /// Gets or sets the agent type. This property is not serialized/deserialized in API calls and can only be set internally.
    /// </summary>
    [JsonIgnore]
    public AgentType? AgentType { get; set; }
}

