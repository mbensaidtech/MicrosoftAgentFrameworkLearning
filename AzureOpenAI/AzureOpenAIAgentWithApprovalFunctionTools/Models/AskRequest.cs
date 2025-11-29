namespace AzureOpenAIAgentWithApprovalFunctionTools.Models;

/// <summary>
/// Request model for asking a question to an agent.
/// </summary>
public class AskRequest
{
    /// <summary>
    /// Gets or sets the question or message to send to the agent.
    /// </summary>
    public string Question { get; set; } = string.Empty;
}

