using System.Text.Json.Serialization;

namespace AzureAIFoundryFileSearch.Models;

/// <summary>
/// Represents an initialized vector store with its associated file and store IDs.
/// </summary>
public class InitializedVectorStore
{
    /// <summary>
    /// Gets or sets the name of the vector store (from configuration).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file ID that was uploaded and associated with the vector store.
    /// </summary>
    [JsonPropertyName("fileId")]
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vector store ID that was created.
    /// </summary>
    [JsonPropertyName("vectorStoreId")]
    public string VectorStoreId { get; set; } = string.Empty;
}

