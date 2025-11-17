using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using Azure.Identity;
using Microsoft.Agents.AI;
using Azure.AI.Agents.Persistent;
using Azure;
using static CommonUtilities.ColoredConsole;
using System.Linq;

namespace AzureAIFoundryShared;

/// <summary>
/// Facade for persistent agents client operations.
/// Provides a unified interface for managing agents, vector stores, and files.
/// </summary>
public class PersistentAgentsClientFacade : IPersistentAgentsClientFacade
{
    private readonly AgentConfig _agentConfig;
    private readonly PersistentAgentsClient _persistentAgentsClient;
    private readonly PersistentAgentsAdministrationClient _persistentAgentsAdministrationClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentAgentsClientFacade"/> class.
    /// </summary>
    /// <param name="agentConfig">The agent configuration.</param>
    public PersistentAgentsClientFacade(AgentConfig agentConfig)
    {
        _agentConfig = agentConfig ?? throw new ArgumentNullException(nameof(agentConfig));

        // Initialize Azure OpenAI client
        var credential = new DefaultAzureCredential(
            new DefaultAzureCredentialOptions { ExcludeAzureDeveloperCliCredential = false });

        WriteSecondaryLogLine($"Endpoint: {_agentConfig.GetEndpoint()}");
        _persistentAgentsClient = new(_agentConfig.GetEndpoint(), credential);
        _persistentAgentsAdministrationClient = _persistentAgentsClient.Administration;
    }

    /// <summary>
    /// Gets an agent by its ID, or creates a new agent if the ID is not provided.
    /// </summary>
    /// <param name="agentId">The ID of the agent to retrieve. If null, a new agent will be created.</param>
    /// <param name="agentType">The type of agent to create. Required when agentId is null.</param>
    /// <returns>The agent.</returns>
    public async Task<AIAgent> GetOrCreateAgentAsync(string? agentId, AgentConfiguration.AgentType? agentType)
    {
        if (!string.IsNullOrWhiteSpace(agentId))
        {
            return await GetAgentByIdAsync(agentId);
        }

        if (agentType == null)
        {
            throw new ArgumentNullException(nameof(agentType), "AgentType is required when agentId is not provided.");
        }

        var createAgentRequest = CreateAgentRequest(agentType.Value);
        return await CreateAgentAsync(createAgentRequest);
    }

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
    /// Creates a vector store in azure ai foundry.
    /// </summary>
    /// <param name="name">The name of the vector store.</param>
    /// <param name="description">Optional description for the vector store.</param>
    /// <returns>The response from the vector store creation.</returns>
    public async Task<Response<PersistentAgentsVectorStore>> CreateVectorStoreAsync(string name, string? description = null)
    {
        // Check if the API supports description parameter
        // If the SDK supports it, we'll use it; otherwise, we'll just use the name
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
        
        // Use existing vector store if ID is provided, otherwise create a new one
        if (!string.IsNullOrWhiteSpace(request.VectorStoreId))
        {
            // Get existing vector store
            vectorStore = await _persistentAgentsClient.VectorStores.GetVectorStoreAsync(request.VectorStoreId);
            
            // Clean the vector store before uploading new file if requested
            if (request.CleanVectorStore)
            {
                await CleanVectorStoreAsync(request.VectorStoreId, request.CleanVectorStoreAndRemoveFilesFromDatasets);
            }
        }
        else
        {
            // Create new vector store - VectorStoreName is required
            if (string.IsNullOrWhiteSpace(request.VectorStoreName))
            {
                throw new ArgumentException("VectorStoreName is required when creating a new vector store.", nameof(request));
            }
            vectorStore = await CreateVectorStoreAsync(request.VectorStoreName, request.VectorStoreDescription);
        }

        // Upload all files and add them to the vector store
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

        // Always use the name from the vector store to ensure accuracy
        return new Models.VectorStoreInitializationResult
        {
            VectorStoreName = vectorStore.Value.Name ?? string.Empty,
            Files = files,
            VectorStoreId = vectorStore.Value.Id
        };
    }

    /// <summary>
    /// Creates an agent with the specified parameters.
    /// </summary>
    /// <param name="request">The request containing agent creation parameters.</param>
    /// <returns>The created agent.</returns>
    private async Task<AIAgent> CreateAgentAsync(CreateAgentRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.DeploymentName))
        {
            throw new ArgumentException("Deployment name cannot be null or empty.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.AgentName))
        {
            throw new ArgumentException("Agent name cannot be null or empty.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Instructions))
        {
            throw new ArgumentException("Instructions cannot be null or empty.", nameof(request));
        }

        Response<PersistentAgent> newPersistentAgent;
        
        // Only include toolResources and tools if both are not null/empty
        bool hasToolResources = request.ToolResources != null;
        bool hasTools = request.Tools != null && request.Tools.Count > 0;
        
        if (hasToolResources && hasTools)
        {
            newPersistentAgent = _persistentAgentsAdministrationClient.CreateAgent(
                model: request.DeploymentName, 
                name: request.AgentName, 
                instructions: request.Instructions, 
                toolResources: request.ToolResources, 
                tools: request.Tools);
        }
        else
        {
            newPersistentAgent = _persistentAgentsAdministrationClient.CreateAgent(
                model: request.DeploymentName, 
                name: request.AgentName, 
                instructions: request.Instructions);
        }
        
        return await _persistentAgentsClient.GetAIAgentAsync(newPersistentAgent.Value.Id);
    }

    /// <summary>
    /// Gets an agent by ID.
    /// </summary>
    /// <param name="agentId">The ID of the agent.</param>
    /// <returns>The agent.</returns>
    private async Task<AIAgent> GetAgentByIdAsync(string agentId)
    {
        if (string.IsNullOrWhiteSpace(agentId))
        {
            throw new ArgumentException("Agent ID cannot be null or empty.", nameof(agentId));
        }

        AIAgent agent = await _persistentAgentsClient.GetAIAgentAsync(agentId);
        return agent;
    }

    /// <summary>
    /// Creates an agent request from the specified agent type.
    /// </summary>
    /// <param name="agentType">The type of agent to create a request for.</param>
    /// <returns>The agent creation request.</returns>
    private CreateAgentRequest CreateAgentRequest(AgentConfiguration.AgentType agentType)
    {
        var agentSettings = _agentConfig.GetAgent(agentType);
        var deploymentName = _agentConfig.GetDeploymentName();

        var (toolResources, tools) = CreateToolResourcesAndTools(agentSettings);

        return new CreateAgentRequest
        {
            DeploymentName = deploymentName,
            AgentName = agentSettings.Name,
            Instructions = agentSettings.Instructions,
            ToolResources = toolResources,
            Tools = tools
        };
    }

    /// <summary>
    /// Creates ToolResources and Tools based on agent settings configuration.
    /// </summary>
    /// <param name="agentSettings">The agent settings containing tools configuration.</param>
    /// <returns>A tuple containing ToolResources and Tools, or null values if not configured.</returns>
    private (ToolResources? toolResources, List<ToolDefinition>? tools) CreateToolResourcesAndTools(AgentConfiguration.AgentSettings agentSettings)
    {
        // If VectorStoreId is configured, create ToolResources and Tools
        if (string.IsNullOrWhiteSpace(agentSettings.Tools?.VectorStores?.VectorStoreId))
        {
            return (null, null);
        }

        var vectorStoreId = agentSettings.Tools.VectorStores.VectorStoreId;
        var maxNumResults = agentSettings.Tools.VectorStores.MaxNumResults;
        
        var toolResources = new ToolResources
        {
            FileSearch = new FileSearchToolResource
            {
                VectorStoreIds = { vectorStoreId }
            }
        };

        var tools = new List<ToolDefinition>
        {
            new FileSearchToolDefinition
            {
                FileSearch = new FileSearchToolDefinitionDetails
                {
                    MaxNumResults = maxNumResults
                }
            }
        };

        return (toolResources, tools);
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

        // Get all files from the vector store
        var files = GetVectorStoreFiles(vectorStoreId);

        // Delete each file
        foreach (var file in files)
        {
            if (!string.IsNullOrWhiteSpace(file.Id))
            {
                if (removeFilesFromDatasets)
                {
                    // First, remove from vector store
                    await DeleteVectorStoreFileAsync(vectorStoreId, file.Id);
                    // Then, remove from Datasets
                    await DeleteFileAsync(file.Id);
                }
                else
                {
                    // Only remove from vector store
                    await DeleteVectorStoreFileAsync(vectorStoreId, file.Id);
                }
            }
        }
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

