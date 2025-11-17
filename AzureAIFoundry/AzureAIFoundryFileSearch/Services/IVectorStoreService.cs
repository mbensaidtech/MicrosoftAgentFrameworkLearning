using Azure.AI.Agents.Persistent;
using Azure;

namespace AzureAIFoundryFileSearch.Services;

/// <summary>
/// Interface for vector store management service.
/// </summary>
public interface IVectorStoreService
{
    /// <summary>
    /// Gets a vector store by ID.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store.</param>
    /// <returns>The vector store information.</returns>
    Task<PersistentAgentsVectorStore> GetVectorStoreAsync(string vectorStoreId);

    /// <summary>
    /// Creates a new vector store.
    /// </summary>
    /// <param name="name">The name of the vector store.</param>
    /// <param name="description">Optional description for the vector store.</param>
    /// <returns>The created vector store.</returns>
    Task<PersistentAgentsVectorStore> CreateVectorStoreAsync(string name, string? description = null);

    /// <summary>
    /// Gets all files from a vector store.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store.</param>
    /// <returns>The list of files in the vector store.</returns>
    Task<List<VectorStoreFile>> GetVectorStoreFilesAsync(string vectorStoreId);

    /// <summary>
    /// Adds a file to a vector store.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store.</param>
    /// <param name="fileId">The ID of the file to add.</param>
    /// <returns>The task representing the asynchronous operation.</returns>
    Task AddFileToVectorStoreAsync(string vectorStoreId, string fileId);

    /// <summary>
    /// Cleans a vector store by removing all files from it.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store to clean.</param>
    /// <param name="removeFilesFromDatasets">If true, files will also be deleted from Azure AI Foundry file storage (Datasets).</param>
    /// <returns>The task representing the asynchronous operation.</returns>
    Task CleanVectorStoreAsync(string vectorStoreId, bool removeFilesFromDatasets = false);
}

