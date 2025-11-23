namespace AzureOpenAIShared.Stores;

/// <summary>
/// Enumeration of available vector store types.
/// </summary>
public enum VectorStoresTypes
{
    /// <summary>
    /// In-memory vector store - stores data in memory.
    /// </summary>
    InMemory,

    /// <summary>
    /// MongoDb vector store - stores data in MongoDb database.
    /// </summary>
    Mongo
}

