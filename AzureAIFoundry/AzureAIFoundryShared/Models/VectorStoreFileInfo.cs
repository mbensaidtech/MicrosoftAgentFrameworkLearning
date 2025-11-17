namespace AzureAIFoundryShared.Models;

/// <summary>
/// Information about a file to be uploaded to a vector store.
/// </summary>
public class VectorStoreFileInfo
{
    /// <summary>
    /// Gets or sets the path to the file to upload.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;
}

