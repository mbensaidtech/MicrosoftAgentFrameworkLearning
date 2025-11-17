using AzureAIFoundryShared;
using Azure.AI.Agents.Persistent;
using Azure;

namespace AzureAIFoundryFileSearch.Services;

/// <summary>
/// Service for dataset (file) management.
/// </summary>
public class DatasetService : IDatasetService
{
    private readonly IPersistentAgentsClientFacade _persistentAgentsClientFacade;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetService"/> class.
    /// </summary>
    /// <param name="persistentAgentsClientFacade">The persistent agents client facade.</param>
    public DatasetService(IPersistentAgentsClientFacade persistentAgentsClientFacade)
    {
        _persistentAgentsClientFacade = persistentAgentsClientFacade ?? throw new ArgumentNullException(nameof(persistentAgentsClientFacade));
    }

    /// <inheritdoc/>
    public async Task<List<PersistentAgentFileInfo>> ListFilesAsync()
    {
        return await _persistentAgentsClientFacade.ListFilesAsync();
    }

    /// <inheritdoc/>
    public async Task<PersistentAgentFileInfo> GetFileAsync(string fileId)
    {
        var response = await _persistentAgentsClientFacade.GetFileAsync(fileId);
        return response.Value;
    }

    /// <inheritdoc/>
    public async Task<PersistentAgentFileInfo> UploadFileAsync(string filePath)
    {
        // Combine the Files folder with the provided file path
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Files", filePath);
        
        var response = await _persistentAgentsClientFacade.UploadFileAsync(fullPath);
        return response.Value;
    }

    /// <inheritdoc/>
    public async Task DeleteFileAsync(string fileId)
    {
        await _persistentAgentsClientFacade.DeleteFileAsync(fileId);
    }
}

