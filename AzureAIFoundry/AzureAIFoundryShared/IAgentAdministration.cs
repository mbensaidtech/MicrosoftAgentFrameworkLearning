using Microsoft.Agents.AI;
using Azure;
using Azure.AI.Agents.Persistent;

namespace AzureAIFoundryShared;

/// <summary>
/// Interface for agent administration service.
/// </summary>
public interface IAgentAdministration
{
    /// <summary>
    /// Gets an agent by its ID, or creates a new agent if the ID is not provided.
    /// </summary>
    /// <param name="agentId">The ID of the agent to retrieve. If null, a new agent will be created.</param>
    /// <param name="agentType">The type of agent to create. Required when agentId is null.</param>
    /// <returns>The agent.</returns>
    Task<AIAgent> GetOrCreateAgentAsync(string? agentId, AgentConfiguration.AgentType? agentType);

    /// <summary>
    /// Uploads a file to azure ai foundry.
    /// </summary>
    /// <param name="filePath">The path to the file to upload.</param>
    /// <param name="fileName">Optional filename to use. If null, the filename from filePath will be used.</param>
    /// <returns>The response from the file upload.</returns>
    Task<Response<PersistentAgentFileInfo>> UploadFileAsync(string filePath, string? fileName = null);

    /// <summary>
    /// Creates a vector store in azure ai foundry.
    /// </summary>
    /// <param name="name">The name of the vector store.</param>
    /// <param name="description">Optional description for the vector store.</param>
    /// <returns>The response from the vector store creation.</returns>
    Task<Response<PersistentAgentsVectorStore>> CreateVectorStoreAsync(string name, string? description = null);

    /// <summary>
    /// Adds a file to a vector store in azure ai foundry.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store.</param>
    /// <param name="fileId">The ID of the file to add.</param>
    /// <returns>The task representing the asynchronous operation.</returns>
    Task AddFileToVectorStoreAsync(string vectorStoreId, string fileId);

    /// <summary>
    /// Initializes a vector store by creating it (or updating if vectorStoreId is provided), uploading a file, and adding the file to the vector store.
    /// </summary>
    /// <param name="request">The request containing vector store name, file path, and optional vector store ID.</param>
    /// <returns>The initialization result containing the vector store name, file ID, and vector store ID.</returns>
    Task<Models.VectorStoreInitializationResult> InitializeVectorStoreAsync(Models.InitializeVectorStoreRequest request);
}

