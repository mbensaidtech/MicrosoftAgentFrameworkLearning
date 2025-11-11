namespace AzureAIFoundryQuickStart.Models;

/// <summary>
/// Context object for conversation continuity.
/// </summary>
public class ConversationContext
{
    /// <summary>
    /// Gets or sets the thread ID for conversation continuity (optional).
    /// </summary>
    public string? ThreadId { get; set; }

    /// <summary>
    /// Gets or sets the agent ID to use (optional).
    /// </summary>
    public string? AgentId { get; set; }
}

