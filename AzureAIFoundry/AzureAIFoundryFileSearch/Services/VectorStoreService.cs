using AzureAIFoundryShared;
using Azure.AI.Agents.Persistent;
using Azure;

namespace AzureAIFoundryFileSearch.Services;

/// <summary>
/// Service for vector store management.
/// </summary>
public class VectorStoreService : IVectorStoreService
{
    private readonly IPersistentAgentsClientFacade _persistentAgentsClientFacade;

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorStoreService"/> class.
    /// </summary>
    /// <param name="persistentAgentsClientFacade">The persistent agents client facade.</param>
    public VectorStoreService(IPersistentAgentsClientFacade persistentAgentsClientFacade)
    {
        _persistentAgentsClientFacade = persistentAgentsClientFacade ?? throw new ArgumentNullException(nameof(persistentAgentsClientFacade));
    }

    /// <inheritdoc/>
    public async Task<PersistentAgentsVectorStore> GetVectorStoreAsync(string vectorStoreId)
    {
        var response = await _persistentAgentsClientFacade.GetVectorStoreAsync(vectorStoreId);
        return response.Value;
    }

    /// <inheritdoc/>
    public async Task<PersistentAgentsVectorStore> CreateVectorStoreAsync(string name, string? description = null)
    {
        var response = await _persistentAgentsClientFacade.CreateVectorStoreAsync(name, description);
        return response.Value;
    }

    /// <inheritdoc/>
    public async Task<List<VectorStoreFile>> GetVectorStoreFilesAsync(string vectorStoreId)
    {
        return await _persistentAgentsClientFacade.GetVectorStoreFilesAsync(vectorStoreId);
    }

    /// <inheritdoc/>
    public async Task AddFileToVectorStoreAsync(string vectorStoreId, string fileId)
    {
        await _persistentAgentsClientFacade.AddFileToVectorStoreAsync(vectorStoreId, fileId);
    }

    /// <inheritdoc/>
    public async Task CleanVectorStoreAsync(string vectorStoreId, bool removeFilesFromDatasets = false)
    {
        await _persistentAgentsClientFacade.CleanVectorStoreAsync(vectorStoreId, removeFilesFromDatasets);
    }
}

