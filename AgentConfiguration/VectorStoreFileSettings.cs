namespace AgentConfiguration;

/// <summary>
/// Configuration settings for a file in a vector store.
/// </summary>
public class VectorStoreFileSettings
{
    /// <summary>
    /// Gets or sets the path to the file that will be used with the vector store.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;
}

