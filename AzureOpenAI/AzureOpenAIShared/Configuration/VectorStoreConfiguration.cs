namespace AzureOpenAIShared.Configuration;

/// <summary>
/// Configuration settings for vector stores.
/// </summary>
public class VectorStoreConfiguration
{
    /// <summary>
    /// Gets or sets the MongoDB configuration settings.
    /// </summary>
    public MongoDbConfiguration MongoDb { get; set; } = new();
}

