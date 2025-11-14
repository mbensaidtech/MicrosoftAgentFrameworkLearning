using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using AzureAIFoundryShared;
using AzureAIFoundryShared.Models;
using Microsoft.Agents.AI;
using AzureAIFoundryFileSearch.Models;
using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using System.Text.Json;
using System.Linq;
using static CommonUtilities.ColoredConsole;

namespace AzureAIFoundryFileSearch.Services;

/// <summary>
/// Service for agent conversations.
/// </summary>
public class AgentConversationService : IAgentConversationService
{
    private readonly IAgentAdministration _agentAdministration;
    private readonly AgentConfig _agentConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentConversationService"/> class.
    /// </summary>
    /// <param name="agentAdministration">The agent administration service.</param>
    /// <param name="agentConfig">The agent configuration.</param>
    public AgentConversationService(IAgentAdministration agentAdministration, AgentConfig agentConfig)
    {
        _agentAdministration = agentAdministration ?? throw new ArgumentNullException(nameof(agentAdministration));
        _agentConfig = agentConfig ?? throw new ArgumentNullException(nameof(agentConfig));
    }

    /// <summary>
    /// Initializes a vector store by name using configuration.
    /// </summary>
    /// <param name="vectorStoreName">The name of the vector store configuration.</param>
    /// <returns>The initialized vector store information.</returns>
    public async Task<Models.InitializedVectorStore> InitVectorStoreAsync(string vectorStoreName)
    {
        var vectorStoreConfig = _agentConfig.GetVectorStore(vectorStoreName);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", vectorStoreConfig.FilePath);
        
        var request = new AzureAIFoundryShared.Models.InitializeVectorStoreRequest
        {
            VectorStoreName = vectorStoreConfig.VectorStoreName, // Can be null if VectorStoreId is provided
            FilePath = filePath,
            FileName = vectorStoreConfig.FileName,
            VectorStoreId = vectorStoreConfig.VectorStoreId,
            VectorStoreDescription = vectorStoreConfig.VectorStoreDescription
        };

        var result = await _agentAdministration.InitializeVectorStoreAsync(request);

        return new Models.InitializedVectorStore
        {
            Name = vectorStoreName,
            FileId = result.FileId,
            VectorStoreId = result.VectorStoreId
        };
    }

    /// <summary>
    /// Initializes all configured vector stores where Initialize is true.
    /// </summary>
    /// <returns>The list of initialized vector stores.</returns>
    public async Task<List<Models.InitializedVectorStore>> InitAllVectorStoresAsync()
    {
        var initializedStores = new List<Models.InitializedVectorStore>();
        
        foreach (var kvp in _agentConfig.VectorStores)
        {
            if (kvp.Value.Initialize)
            {
                var initializedStore = await InitVectorStoreAsync(kvp.Key);
                initializedStores.Add(initializedStore);
            }
        }

        return initializedStores;
    }

    /// <inheritdoc/>
    public async Task<AgentResponse> SendMessageAsync(SendMessageRequest request)
    {
        if (request.Context?.AgentType == null)
        {
            throw new ArgumentException("AgentType must be set in request.Context.AgentType.", nameof(request));
        }

        SendMessageRequest.Validate(request);

        var agentType = request.Context.AgentType.Value;
        var persistentAgent = await _agentAdministration.GetOrCreateAgentAsync(request.Context?.AgentId, agentType);
        var agentThread = await CreateOrResumeAgentThreadAsync(persistentAgent, request.Context?.ThreadId);

        AgentRunResponse agentRunResponse = await persistentAgent.RunAsync(request.Message, agentThread);
        agentRunResponse.LogTokenUsage();
        await SaveThreadStateAsync(persistentAgent.Id, agentThread);

        // Convert AgentRunResponse to AgentResponse using extension method
        return agentRunResponse.ToAgentResponse();
    }

    /// <summary>
    /// Creates a new thread or resumes an existing thread based on the provided thread ID.
    /// </summary>
    /// <param name="agent">The agent to create or resume the thread for.</param>
    /// <param name="threadId">The thread ID to resume. If null or not found in file, creates a new thread.</param>
    /// <returns>The agent thread.</returns>
    private async Task<AgentThread> CreateOrResumeAgentThreadAsync(AIAgent agent, string? threadId)
    {
        if (!string.IsNullOrWhiteSpace(threadId))
        {
            // Try to load from file using ThreadId
            var agentThread = await ResumeThreadFromFileAsync(agent.Id, threadId, agent);
            
            // If found in file, return it
            if (agentThread != null)
            {
                return agentThread;
            }
        }

        // Create a new thread when ThreadId is not provided or not found in file
        return agent.GetNewThread();
    }

    /// <summary>
    /// Gets the directory path for thread state files for a specific agent.
    /// Structure: AgentsThreads/{agentId}/Threads/
    /// </summary>
    /// <param name="agentId">The agent ID.</param>
    /// <returns>The directory path for the agent's threads.</returns>
    private string GetThreadStateDirectory(string agentId)
    {
        string projectRoot = Directory.GetCurrentDirectory();
        return Path.Combine(projectRoot, "AgentsThreads", agentId, "Threads");
    }

    /// <summary>
    /// Gets the file path for storing the thread state based on agentId and threadId.
    /// Structure: AgentsThreads/{agentId}/Threads/{threadId}.json
    /// </summary>
    /// <param name="agentId">The agent ID.</param>
    /// <param name="threadId">The thread ID to use as the filename.</param>
    /// <returns>The file path for the thread state file.</returns>
    private string GetThreadStateFilePath(string agentId, string threadId)
    {
        string directory = GetThreadStateDirectory(agentId);
        string fileName = $"{threadId}.json";
        return Path.Combine(directory, fileName);
    }

    /// <summary>
    /// Serializes and saves the thread state to a local file using threadId as filename.
    /// Structure: AgentsThreads/{agentId}/Threads/{threadId}.json
    /// </summary>
    /// <param name="agentId">The agent ID.</param>
    /// <param name="agentThread">The agent thread to serialize and save.</param>
    /// <remarks>This is an example implementation. Replace with DB or blob storage in production.</remarks>
    private async Task SaveThreadStateAsync(string agentId, AgentThread agentThread)
    {
        string serializedJson = agentThread.Serialize(JsonSerializerOptions.Web).GetRawText();
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(serializedJson, JsonSerializerOptions.Web);
        
        // Extract threadId from the JSON (property name is conversationId in the JSON)
        string? threadId = null;
        if (jsonElement.TryGetProperty("conversationId", out var conversationIdElement))
        {
            threadId = conversationIdElement.GetString();
        }

        if (string.IsNullOrWhiteSpace(threadId))
        {
            // If no threadId, use a default filename
            threadId = "agent_thread";
        }

        // Ensure the directory exists
        string directory = GetThreadStateDirectory(agentId);
        Directory.CreateDirectory(directory);

        string filePath = GetThreadStateFilePath(agentId, threadId);
        await File.WriteAllTextAsync(filePath, serializedJson);
    }

    /// <summary>
    /// Resumes a thread from a persisted JSON file using the provided threadId.
    /// </summary>
    /// <param name="agentId">The agent ID.</param>
    /// <param name="threadId">The thread ID to load.</param>
    /// <param name="agent">The agent to deserialize the thread for.</param>
    /// <returns>The resumed agent thread, or null if the file is not found.</returns>
    /// <remarks>This is an example implementation. Replace with DB or blob storage in production.</remarks>
    private async Task<AgentThread?> ResumeThreadFromFileAsync(string agentId, string threadId, AIAgent agent)
    {
        string directory = GetThreadStateDirectory(agentId);
        
        if (!Directory.Exists(directory))
        {
            return null;
        }

        string filePath = GetThreadStateFilePath(agentId, threadId);
        
        if (!File.Exists(filePath))
        {
            return null;
        }

        try
        {
            string loadedJson = await File.ReadAllTextAsync(filePath);
            JsonElement reloaded = JsonSerializer.Deserialize<JsonElement>(loadedJson, JsonSerializerOptions.Web);
            AgentThread resumedThread = agent.DeserializeThread(reloaded, JsonSerializerOptions.Web);
            return resumedThread;
        }
        catch
        {
            return null;
        }
    }
}

