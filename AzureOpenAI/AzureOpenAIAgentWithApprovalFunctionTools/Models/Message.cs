using System.Text.Json.Serialization;

namespace AzureOpenAIAgentWithApprovalFunctionTools.Models;

/// <summary>
/// Represents a message in a conversation.
/// </summary>
public class Message
{
    /// <summary>
    /// Gets or sets the name of the message author.
    /// </summary>
    [JsonPropertyName("authorName")]
    public string AuthorName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the message was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the role of the message.
    /// </summary>
    [JsonPropertyName("role")]
    public MessageRole Role { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the message.
    /// </summary>
    [JsonPropertyName("messageId")]
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of the message.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

