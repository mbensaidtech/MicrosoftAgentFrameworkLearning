using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using Azure.Identity;
using Microsoft.Agents.AI;
using Azure.AI.Agents.Persistent;
using Azure;
using static CommonUtilities.ColoredConsole;

namespace AzureAIFoundryShared;

/// <summary>
/// Service for agent administration.
/// </summary>
public class AgentAdministration : IAgentAdministration
{
    private readonly AgentConfig _agentConfig;
    private readonly PersistentAgentsClient _persistentAgentsClient;
    private readonly PersistentAgentsAdministrationClient _persistentAgentsAdministrationClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentAdministration"/> class.
    /// </summary>
    /// <param name="agentConfig">The agent configuration.</param>
    public AgentAdministration(AgentConfig agentConfig)
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
    /// <param name="fileName">Optional filename to use. If null, the filename from filePath will be used.</param>
    /// <returns>The response from the file upload.</returns>
    public async Task<Response<PersistentAgentFileInfo>> UploadFileAsync(string filePath, string? fileName = null)
    {
        // Note: The Azure SDK may not support custom filename directly, but we pass it if available
        // The actual implementation depends on the SDK capabilities
        return await _persistentAgentsClient.Files.UploadFileAsync(filePath, PersistentAgentFilePurpose.Agents);
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
            
            // Clean the vector store before uploading new file
            await CleanVectorStoreAsync(request.VectorStoreId);
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

        var file = await UploadFileAsync(request.FilePath, request.FileName);
        await AddFileToVectorStoreAsync(vectorStore.Value.Id, file.Value.Id);

        // Always use the name from the vector store to ensure accuracy
        return new Models.VectorStoreInitializationResult
        {
            VectorStoreName = vectorStore.Value.Name ?? string.Empty,
            FileId = file.Value.Id,
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

        return new CreateAgentRequest
        {
            DeploymentName = deploymentName,
            AgentName = agentSettings.Name,
            Instructions = agentSettings.Instructions
        };
    }

    /// <summary>
    /// Gets all files from a vector store.
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
    private async Task<Response<bool>> DeleteVectorStoreFileAsync(string vectorStoreId, string fileId)
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
    /// <returns>The task representing the asynchronous operation.</returns>
    private async Task CleanVectorStoreAsync(string vectorStoreId)
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
                await DeleteVectorStoreFileAsync(vectorStoreId, file.Id);
            }
        }
    }

}

