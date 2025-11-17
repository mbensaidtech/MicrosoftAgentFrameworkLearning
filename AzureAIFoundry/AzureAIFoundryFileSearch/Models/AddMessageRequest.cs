namespace AzureAIFoundryFileSearch.Models;

/// <summary>
/// Request model for adding a message to an existing conversation.
/// </summary>
public class AddMessageRequest
{
    /// <summary>
    /// Gets or sets the ID of the agent to use for this message.
    /// </summary>
    public string AgentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message to send.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

