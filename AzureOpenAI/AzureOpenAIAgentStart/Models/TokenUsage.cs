using System.Text.Json.Serialization;

namespace AzureOpenAIAgentStart.Models;

/// <summary>
/// Represents token usage information for an AI operation.
/// </summary>
public class TokenUsage
{
    /// <summary>
    /// Gets or sets the number of input tokens used.
    /// </summary>
    [JsonPropertyName("inputTokenCount")]
    public int InputTokenCount { get; set; }

    /// <summary>
    /// Gets or sets the number of output tokens used.
    /// </summary>
    [JsonPropertyName("outputTokenCount")]
    public int OutputTokenCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of tokens used.
    /// </summary>
    [JsonPropertyName("totalTokenCount")]
    public int TotalTokenCount { get; set; }

    /// <summary>
    /// Gets or sets additional token count information (optional).
    /// </summary>
    [JsonPropertyName("additionalCounts")]
    public Dictionary<string, int>? AdditionalCounts { get; set; }
}

