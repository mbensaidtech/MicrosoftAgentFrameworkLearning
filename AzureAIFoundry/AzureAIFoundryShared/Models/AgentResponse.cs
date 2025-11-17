using System.Text.Json.Serialization;

namespace AzureAIFoundryShared.Models;

/// <summary>
/// Represents the response from an agent conversation.
/// </summary>
public class AgentResponse
{
    /// <summary>
    /// Gets or sets the agent ID.
    /// </summary>
    [JsonPropertyName("agentId")]
    public string AgentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the thread ID (conversation ID).
    /// </summary>
    [JsonPropertyName("threadId")]
    public string ThreadId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the response was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the list of messages in the response.
    /// </summary>
    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; } = new();

    /// <summary>
    /// Gets or sets the token usage information.
    /// </summary>
    [JsonPropertyName("usage")]
    public TokenUsage? Usage { get; set; }
}

