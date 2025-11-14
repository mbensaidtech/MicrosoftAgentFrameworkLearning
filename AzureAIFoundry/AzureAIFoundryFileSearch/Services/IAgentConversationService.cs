using AgentConfiguration;
using AzureAIFoundryShared.Models;
using AzureAIFoundryFileSearch.Models;

namespace AzureAIFoundryFileSearch.Services;

/// <summary>
/// Interface for agent conversation service.
/// </summary>
public interface IAgentConversationService
{
    /// <summary>
    /// Sends a message to an agent and gets a response. The agent type is read from request.Context.AgentType.
    /// </summary>
    /// <param name="request">The request containing the message, optional thread ID, and agent type in context.</param>
    /// <returns>The agent's response.</returns>
    Task<AgentResponse> SendMessageAsync(SendMessageRequest request);

    /// <summary>
    /// Initializes a vector store by name using configuration.
    /// </summary>
    /// <param name="vectorStoreName">The name of the vector store configuration.</param>
    /// <returns>The initialized vector store information.</returns>
    Task<Models.InitializedVectorStore> InitVectorStoreAsync(string vectorStoreName);

    /// <summary>
    /// Initializes all configured vector stores where Initialize is true.
    /// </summary>
    /// <returns>The list of initialized vector stores.</returns>
    Task<List<Models.InitializedVectorStore>> InitAllVectorStoresAsync();
}

