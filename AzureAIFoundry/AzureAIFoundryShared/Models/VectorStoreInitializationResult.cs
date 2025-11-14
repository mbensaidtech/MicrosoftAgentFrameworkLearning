namespace AzureAIFoundryShared.Models;

/// <summary>
/// Represents the result of initializing a vector store with a file.
/// </summary>
public class VectorStoreInitializationResult
{
    /// <summary>
    /// Gets or sets the name of the vector store.
    /// </summary>
    public string VectorStoreName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file ID that was uploaded and associated with the vector store.
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vector store ID that was created.
    /// </summary>
    public string VectorStoreId { get; set; } = string.Empty;
}

