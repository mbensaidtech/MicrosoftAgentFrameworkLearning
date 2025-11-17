namespace AzureAIFoundryShared.Models;

/// <summary>
/// Request model for initializing a vector store.
/// </summary>
public class InitializeVectorStoreRequest
{
    /// <summary>
    /// Gets or sets the name of the vector store to create. Required when creating a new store. Not needed when VectorStoreId is provided.
    /// </summary>
    public string? VectorStoreName { get; set; }

    /// <summary>
    /// Gets or sets the list of files to upload and associate with the vector store.
    /// </summary>
    public List<VectorStoreFileInfo> Files { get; set; } = new List<VectorStoreFileInfo>();

    /// <summary>
    /// Gets or sets the vector store ID if updating an existing store. If null, a new store will be created.
    /// </summary>
    public string? VectorStoreId { get; set; }

    /// <summary>
    /// Gets or sets the optional description for the vector store. Used when creating a new store.
    /// </summary>
    public string? VectorStoreDescription { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to clean the vector store (remove all existing files).
    /// </summary>
    public bool CleanVectorStore { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to remove files from Datasets when cleaning the vector store.
    /// If true, files will be deleted from Azure AI Foundry file storage (Datasets) in addition to being removed from the vector store.
    /// </summary>
    public bool CleanVectorStoreAndRemoveFilesFromDatasets { get; set; } = false;
}

