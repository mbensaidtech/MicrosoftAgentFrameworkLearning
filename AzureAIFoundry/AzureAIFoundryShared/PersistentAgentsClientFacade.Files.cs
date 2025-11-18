using Azure;
using Azure.AI.Agents.Persistent;
using System.Linq;

namespace AzureAIFoundryShared;

/// <summary>
/// Partial class for file-related operations.
/// </summary>
public partial class PersistentAgentsClientFacade
{
    /// <summary>
    /// Uploads a file to azure ai foundry.
    /// </summary>
    /// <param name="filePath">The path to the file to upload.</param>
    /// <returns>The response from the file upload.</returns>
    public async Task<Response<PersistentAgentFileInfo>> UploadFileAsync(string filePath)
    {
        string fileName = Path.GetFileName(filePath);
        using var fileStream = File.OpenRead(filePath);
        return await _persistentAgentsClient.Files.UploadFileAsync(fileStream, PersistentAgentFilePurpose.Agents, fileName);
    }

    /// <summary>
    /// Gets a file by ID.
    /// </summary>
    /// <param name="fileId">The ID of the file.</param>
    /// <returns>The file information.</returns>
    public async Task<Response<PersistentAgentFileInfo>> GetFileAsync(string fileId)
    {
        if (string.IsNullOrWhiteSpace(fileId))
        {
            throw new ArgumentException("File ID cannot be null or empty.", nameof(fileId));
        }

        return await _persistentAgentsClient.Files.GetFileAsync(fileId);
    }

    /// <summary>
    /// Lists all files.
    /// </summary>
    /// <returns>A list of files.</returns>
    public async Task<List<PersistentAgentFileInfo>> ListFilesAsync()
    {
        var response = await _persistentAgentsClient.Files.GetFilesAsync();
        return response.Value?.ToList() ?? new List<PersistentAgentFileInfo>();
    }

    /// <summary>
    /// Deletes a file from Azure AI Foundry file storage (Datasets).
    /// </summary>
    /// <param name="fileId">The ID of the file to delete.</param>
    /// <returns>The task representing the asynchronous operation.</returns>
    public async Task DeleteFileAsync(string fileId)
    {
        if (string.IsNullOrWhiteSpace(fileId))
        {
            throw new ArgumentException("File ID cannot be null or empty.", nameof(fileId));
        }

        await _persistentAgentsClient.Files.DeleteFileAsync(fileId);
    }
}

