using System.Text.Json.Serialization;
using AzureAIFoundryShared.Models;

namespace AzureAIFoundrySO.Models;

/// <summary>
/// Represents a message in a conversation with structured output.
/// </summary>
/// <typeparam name="T">The type of structured output.</typeparam>
public class StructuredMessage<T>
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
    /// Gets or sets the structured output for this message.
    /// </summary>
    [JsonPropertyName("structuredOutput")]
    public T? StructuredOutput { get; set; }
}

