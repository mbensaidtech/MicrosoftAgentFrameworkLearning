using Azure.AI.Agents.Persistent;

namespace AzureAIFoundryFileSearch.Services;

/// <summary>
/// Interface for dataset (file) management service.
/// </summary>
public interface IDatasetService
{
    /// <summary>
    /// Lists all files in the Datasets.
    /// </summary>
    /// <returns>The list of files.</returns>
    Task<List<PersistentAgentFileInfo>> ListFilesAsync();

    /// <summary>
    /// Gets a file by ID.
    /// </summary>
    /// <param name="fileId">The ID of the file.</param>
    /// <returns>The file information.</returns>
    Task<PersistentAgentFileInfo> GetFileAsync(string fileId);

    /// <summary>
    /// Uploads a file to Datasets.
    /// </summary>
    /// <param name="filePath">The path to the file to upload.</param>
    /// <returns>The uploaded file information.</returns>
    Task<PersistentAgentFileInfo> UploadFileAsync(string filePath);

    /// <summary>
    /// Deletes a file from Datasets.
    /// </summary>
    /// <param name="fileId">The ID of the file to delete.</param>
    /// <returns>The task representing the asynchronous operation.</returns>
    Task DeleteFileAsync(string fileId);
}

