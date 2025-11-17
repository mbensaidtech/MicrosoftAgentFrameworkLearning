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
    /// Gets or sets the list of files that were uploaded and associated with the vector store.
    /// </summary>
    [JsonPropertyName("files")]
    public List<VectorStoreFileInfo> Files { get; set; } = new List<VectorStoreFileInfo>();

    /// <summary>
    /// Gets or sets the vector store ID that was created.
    /// </summary>
    [JsonPropertyName("vectorStoreId")]
    public string VectorStoreId { get; set; } = string.Empty;
}

