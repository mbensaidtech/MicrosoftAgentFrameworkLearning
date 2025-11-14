namespace AgentConfiguration;

/// <summary>
/// Configuration settings for a vector store.
/// </summary>
public class VectorStoreSettings
{
    /// <summary>
    /// Gets or sets the name of the vector store. Required when creating a new store. Not needed when VectorStoreId is provided.
    /// </summary>
    public string? VectorStoreName { get; set; }

    /// <summary>
    /// Gets or sets the path to the file that will be used with the vector store.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional filename to use when uploading the file. If null, the filename from FilePath will be used.
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the vector store should be initialized.
    /// </summary>
    public bool Initialize { get; set; } = false;

    /// <summary>
    /// Gets or sets the vector store ID if updating an existing store. If null, a new store will be created.
    /// </summary>
    public string? VectorStoreId { get; set; }

    /// <summary>
    /// Gets or sets the optional description for the vector store. Used when creating a new store.
    /// </summary>
    public string? VectorStoreDescription { get; set; }
}

