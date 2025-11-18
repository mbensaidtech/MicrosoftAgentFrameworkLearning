using System.Text.Json.Serialization;

namespace AzureAIFoundrySO.Models;

/// <summary>
/// Represents a single book recommendation with structured information.
/// </summary>
public class BookRecommendation
{
    /// <summary>
    /// Gets or sets the title of the book.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author of the book.
    /// </summary>
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the genre of the book.
    /// </summary>
    [JsonPropertyName("genre")]
    public string Genre { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a brief description of the book.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rating of the book (0-5 scale).
    /// </summary>
    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    /// <summary>
    /// Gets or sets the publication date in YYYY-MM-DD format.
    /// </summary>
    [JsonPropertyName("publicationDate")]
    public string PublicationDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ISBN of the book (optional).
    /// </summary>
    [JsonPropertyName("isbn")]
    public string? Isbn { get; set; }

    /// <summary>
    /// Gets or sets the page count of the book (optional).
    /// </summary>
    [JsonPropertyName("pageCount")]
    public int? PageCount { get; set; }

    /// <summary>
    /// Gets or sets the explanation of why this book is recommended.
    /// </summary>
    [JsonPropertyName("whyRecommended")]
    public string WhyRecommended { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a list of similar book titles.
    /// </summary>
    [JsonPropertyName("similarBooks")]
    public List<string> SimilarBooks { get; set; } = new();
}

