using AgentConfiguration;

namespace AzureOpenAIAgentWithThreads.Models;

/// <summary>
/// Request model for asking a question to an agent.
/// </summary>
public class AskRequest
{
    /// <summary>
    /// Gets or sets the question or message to send to the agent.
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional agent type. If not provided, a basic agent will be used.
    /// </summary>
    public AgentType? AgentType { get; set; }

    /// <summary>
    /// Gets or sets the optional thread ID to continue an existing conversation.
    /// </summary>
    public string? ThreadId { get; set; }
}

