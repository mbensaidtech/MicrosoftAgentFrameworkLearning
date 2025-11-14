using System.Text.Json.Serialization;

namespace AzureAIFoundryFileSearch.Models;

/// <summary>
/// Response model for vector store initialization containing a list of initialized stores.
/// </summary>
public class InitVectorStoresResponse
{
    /// <summary>
    /// Gets or sets the list of initialized vector stores.
    /// </summary>
    [JsonPropertyName("initializedStores")]
    public List<InitializedVectorStore> InitializedStores { get; set; } = new List<InitializedVectorStore>();
}

