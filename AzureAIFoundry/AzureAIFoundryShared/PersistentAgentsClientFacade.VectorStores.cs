using Azure;
using Azure.AI.Agents.Persistent;

namespace AzureAIFoundryShared;

/// <summary>
/// Partial class for vector store-related operations.
/// </summary>
public partial class PersistentAgentsClientFacade
{
    /// <summary>
    /// Creates a vector store in azure ai foundry.
    /// </summary>
    /// <param name="name">The name of the vector store.</param>
    /// <param name="description">Optional description for the vector store.</param>
    /// <returns>The response from the vector store creation.</returns>
    public async Task<Response<PersistentAgentsVectorStore>> CreateVectorStoreAsync(string name, string? description = null)
    {
        Response<PersistentAgentsVectorStore> vectorStore = await _persistentAgentsClient.VectorStores.CreateVectorStoreAsync(name: name);
        return vectorStore;
    }

    /// <summary>
    /// Adds a file to a vector store in azure ai foundry.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store.</param>
    /// <param name="fileId">The ID of the file to add.</param>
    /// <returns>The response from the file addition.</returns>
    public async Task AddFileToVectorStoreAsync(string vectorStoreId, string fileId)
    {
        await _persistentAgentsClient.VectorStores.CreateVectorStoreFileAsync(vectorStoreId, fileId);
    }

    /// <summary>
    /// Initializes a vector store by creating it (or updating if vectorStoreId is provided), uploading a file, and adding the file to the vector store.
    /// </summary>
    /// <param name="request">The request containing vector store name, file path, and optional vector store ID.</param>
    /// <returns>The initialization result containing the vector store name, file ID, and vector store ID.</returns>
    public async Task<Models.VectorStoreInitializationResult> InitializeVectorStoreAsync(Models.InitializeVectorStoreRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        Response<PersistentAgentsVectorStore> vectorStore;
        
        if (!string.IsNullOrWhiteSpace(request.VectorStoreId))
        {
            vectorStore = await _persistentAgentsClient.VectorStores.GetVectorStoreAsync(request.VectorStoreId);
            
            if (request.CleanVectorStore)
            {
                await CleanVectorStoreAsync(request.VectorStoreId, request.CleanVectorStoreAndRemoveFilesFromDatasets);
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.VectorStoreName))
            {
                throw new ArgumentException("VectorStoreName is required when creating a new vector store.", nameof(request));
            }
            vectorStore = await CreateVectorStoreAsync(request.VectorStoreName, request.VectorStoreDescription);
        }

        var files = new List<Models.VectorStoreFileResult>();
        foreach (var fileInfo in request.Files)
        {
            var file = await UploadFileAsync(fileInfo.FilePath);
            await AddFileToVectorStoreAsync(vectorStore.Value.Id, file.Value.Id);
            
            files.Add(new Models.VectorStoreFileResult
            {
                FileId = file.Value.Id,
                FileName = Path.GetFileName(fileInfo.FilePath)
            });
        }

        return new Models.VectorStoreInitializationResult
        {
            VectorStoreName = vectorStore.Value.Name ?? string.Empty,
            Files = files,
            VectorStoreId = vectorStore.Value.Id
        };
    }

    /// <summary>
    /// Gets a vector store by ID.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store.</param>
    /// <returns>The vector store.</returns>
    public async Task<Response<PersistentAgentsVectorStore>> GetVectorStoreAsync(string vectorStoreId)
    {
        if (string.IsNullOrWhiteSpace(vectorStoreId))
        {
            throw new ArgumentException("Vector store ID cannot be null or empty.", nameof(vectorStoreId));
        }

        return await _persistentAgentsClient.VectorStores.GetVectorStoreAsync(vectorStoreId);
    }

    /// <summary>
    /// Gets all files from a vector store.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store.</param>
    /// <returns>A list of vector store files.</returns>
    public Task<List<VectorStoreFile>> GetVectorStoreFilesAsync(string vectorStoreId)
    {
        if (string.IsNullOrWhiteSpace(vectorStoreId))
        {
            throw new ArgumentException("Vector store ID cannot be null or empty.", nameof(vectorStoreId));
        }

        var files = new List<VectorStoreFile>();
        Pageable<VectorStoreFile> pageableFiles = _persistentAgentsClient.VectorStores.GetVectorStoreFiles(vectorStoreId);
        
        foreach (var file in pageableFiles)
        {
            files.Add(file);
        }

        return Task.FromResult(files);
    }

    /// <summary>
    /// Gets all files from a vector store (private method for internal use).
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store.</param>
    /// <returns>A list of vector store files.</returns>
    private List<VectorStoreFile> GetVectorStoreFiles(string vectorStoreId)
    {
        if (string.IsNullOrWhiteSpace(vectorStoreId))
        {
            throw new ArgumentException("Vector store ID cannot be null or empty.", nameof(vectorStoreId));
        }

        var files = new List<VectorStoreFile>();
        Pageable<VectorStoreFile> pageableFiles = _persistentAgentsClient.VectorStores.GetVectorStoreFiles(vectorStoreId);
        
        foreach (var file in pageableFiles)
        {
            files.Add(file);
        }

        return files;
    }

    /// <summary>
    /// Deletes a file from a vector store.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store.</param>
    /// <param name="fileId">The ID of the file to delete.</param>
    /// <returns>The response indicating whether the file was deleted.</returns>
    public async Task<Response<bool>> DeleteVectorStoreFileAsync(string vectorStoreId, string fileId)
    {
        if (string.IsNullOrWhiteSpace(vectorStoreId))
        {
            throw new ArgumentException("Vector store ID cannot be null or empty.", nameof(vectorStoreId));
        }

        if (string.IsNullOrWhiteSpace(fileId))
        {
            throw new ArgumentException("File ID cannot be null or empty.", nameof(fileId));
        }

        return await _persistentAgentsClient.VectorStores.DeleteVectorStoreFileAsync(vectorStoreId, fileId);
    }

    /// <summary>
    /// Cleans a vector store by deleting all files from it.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store to clean.</param>
    /// <param name="removeFilesFromDatasets">If true, files will also be deleted from Azure AI Foundry file storage (Datasets).</param>
    /// <returns>The task representing the asynchronous operation.</returns>
    public async Task CleanVectorStoreAsync(string vectorStoreId, bool removeFilesFromDatasets = false)
    {
        if (string.IsNullOrWhiteSpace(vectorStoreId))
        {
            throw new ArgumentException("Vector store ID cannot be null or empty.", nameof(vectorStoreId));
        }

        var files = GetVectorStoreFiles(vectorStoreId);

        foreach (var file in files)
        {
            if (!string.IsNullOrWhiteSpace(file.Id))
            {
                if (removeFilesFromDatasets)
                {
                    await DeleteVectorStoreFileAsync(vectorStoreId, file.Id);
                    await DeleteFileAsync(file.Id);
                }
                else
                {
                    await DeleteVectorStoreFileAsync(vectorStoreId, file.Id);
                }
            }
        }
    }
}

