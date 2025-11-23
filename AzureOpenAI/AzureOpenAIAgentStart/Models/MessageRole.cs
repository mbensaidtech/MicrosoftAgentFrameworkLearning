using System.Text.Json.Serialization;

namespace AzureOpenAIAgentStart.Models;

/// <summary>
/// Represents the role of a message in a conversation.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MessageRole
{
    Assistant,
    System,
    Tool,
    User
}

