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
/// Service for thread management and conversations.
/// </summary>
public class ThreadService : IThreadService
{
    private readonly IPersistentAgentsClientFacade _persistentAgentsClientFacade;
    private readonly AgentConfig _agentConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadService"/> class.
    /// </summary>
    /// <param name="persistentAgentsClientFacade">The persistent agents client facade.</param>
    /// <param name="agentConfig">The agent configuration.</param>
    public ThreadService(IPersistentAgentsClientFacade persistentAgentsClientFacade, AgentConfig agentConfig)
    {
        _persistentAgentsClientFacade = persistentAgentsClientFacade ?? throw new ArgumentNullException(nameof(persistentAgentsClientFacade));
        _agentConfig = agentConfig ?? throw new ArgumentNullException(nameof(agentConfig));
    }


    /// <inheritdoc/>
    public async Task<AgentResponse> SendMessageAsync(SendMessageRequest request)
    {
        SendMessageRequest.Validate(request);

        if (string.IsNullOrWhiteSpace(request.Context?.AgentId) && request.Context?.AgentType == null)
        {
            throw new ArgumentException("AgentType must be set when agentId is not provided.", nameof(request));
        }

        var agentType = request.Context?.AgentType;
        var persistentAgent = await _persistentAgentsClientFacade.GetOrCreateAgentAsync(request.Context?.AgentId, agentType);
        var agentThread = await CreateOrResumeAgentThreadAsync(persistentAgent, request.Context?.ThreadId);

        AgentRunResponse agentRunResponse = await persistentAgent.RunAsync(request.Message, agentThread);
        agentRunResponse.LogTokenUsage();
        await SaveThreadStateAsync(persistentAgent.Id, agentThread);

        return agentRunResponse.ToAgentResponse(agentThread);
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
            var agentThread = await ResumeThreadFromFileAsync(agent.Id, threadId, agent);
            
            if (agentThread != null)
            {
                return agentThread;
            }
        }

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
        return Path.Combine(directory, $"{threadId}.json");
    }

    /// <summary>
    /// Saves the thread state to a JSON file for persistence.
    /// </summary>
    /// <param name="agentId">The agent ID.</param>
    /// <param name="agentThread">The agent thread to save.</param>
    /// <remarks>This is an example implementation. Replace with DB or blob storage in production.</remarks>
    private async Task SaveThreadStateAsync(string agentId, AgentThread agentThread)
    {
        string serializedJson = agentThread.Serialize(JsonSerializerOptions.Web).GetRawText();
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(serializedJson, JsonSerializerOptions.Web);
        
        string? threadId = null;
        if (jsonElement.TryGetProperty("conversationId", out var conversationIdElement))
        {
            threadId = conversationIdElement.GetString();
        }

        if (string.IsNullOrWhiteSpace(threadId))
        {
            threadId = "agent_thread";
        }

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
        catch (Exception ex)
        {
            WriteSecondaryLogLine($"Failed to resume thread {threadId} for agent {agentId}: {ex.Message}");
            return null;
        }
    }
}

