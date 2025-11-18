using System.Text.Json.Serialization;

namespace AzureAIFoundrySO.Models;

/// <summary>
/// Represents a structured response containing book recommendations.
/// </summary>
public class BookRecommendationResponse
{
    /// <summary>
    /// Gets or sets the list of book recommendations.
    /// </summary>
    [JsonPropertyName("recommendations")]
    public List<BookRecommendation> Recommendations { get; set; } = new();

    /// <summary>
    /// Gets or sets the total number of recommendations.
    /// </summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount => Recommendations.Count;

    /// <summary>
    /// Gets or sets any additional context or notes about the recommendations.
    /// </summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}

