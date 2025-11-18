namespace AzureAIFoundrySO.Models;

/// <summary>
/// Request model for creating a new thread for an existing agent.
/// </summary>
public class CreateThreadRequest
{
    /// <summary>
    /// Gets or sets the ID of the agent to use for this thread.
    /// </summary>
    public string AgentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message to send.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

