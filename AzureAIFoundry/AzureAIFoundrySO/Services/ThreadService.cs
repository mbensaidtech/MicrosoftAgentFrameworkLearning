using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using AzureAIFoundryShared;
using AzureAIFoundryShared.Models;
using AzureAIFoundrySO.Models;
using Microsoft.Agents.AI;
using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using System.Text.Json;
using System.Linq;
using static CommonUtilities.ColoredConsole;

namespace AzureAIFoundrySO.Services;

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
    public async Task<StructuredAgentResponse<T>> SendMessageAsync<T>(SendMessageRequest request)
    {
        SendMessageRequest.Validate(request);

        if (string.IsNullOrWhiteSpace(request.Context?.AgentId) && request.Context?.AgentType == null)
        {
            throw new ArgumentException("AgentType must be set when agentId is not provided.", nameof(request));
        }

        var agentType = request.Context?.AgentType;
        var persistentAgent = await _persistentAgentsClientFacade.GetOrCreateChatClientAgentAsync(request.Context?.AgentId, agentType);
        var agentThread = await CreateOrResumeAgentThreadAsync(persistentAgent, request.Context?.ThreadId);

        AgentRunResponse<T> agentRunResponse = await persistentAgent.RunAsync<T>(request.Message, agentThread);
        agentRunResponse.LogTokenUsage();
        await SaveThreadStateAsync(persistentAgent.Id, agentThread);

        var structuredOutput = agentRunResponse.Result;
        
        return ConvertToStructuredAgentResponse(agentRunResponse, agentThread, structuredOutput);
    }

    /// <summary>
    /// Converts an AgentRunResponse with structured output type T to StructuredAgentResponse.
    /// </summary>
    /// <typeparam name="T">The type of structured output.</typeparam>
    /// <param name="agentRunResponse">The agent run response.</param>
    /// <param name="agentThread">The agent thread.</param>
    /// <param name="structuredOutput">The structured output result.</param>
    /// <returns>A StructuredAgentResponse containing the converted data.</returns>
    private StructuredAgentResponse<T> ConvertToStructuredAgentResponse<T>(AgentRunResponse<T> agentRunResponse, AgentThread agentThread, T? structuredOutput)
    {
        var response = new StructuredAgentResponse<T>
        {
            AgentId = agentRunResponse.AgentId ?? string.Empty,
            CreatedAt = agentRunResponse.CreatedAt?.DateTime ?? DateTime.UtcNow,
            Usage = agentRunResponse.Usage != null ? new TokenUsage
            {
                InputTokenCount = (int)(agentRunResponse.Usage.InputTokenCount ?? 0),
                OutputTokenCount = (int)(agentRunResponse.Usage.OutputTokenCount ?? 0),
                TotalTokenCount = (int)(agentRunResponse.Usage.TotalTokenCount ?? 0),
                AdditionalCounts = agentRunResponse.Usage.AdditionalCounts != null
                    ? agentRunResponse.Usage.AdditionalCounts.ToDictionary(kvp => kvp.Key, kvp => (int)kvp.Value)
                    : null
            } : null,
            Messages = agentRunResponse.Messages?.Select(msg => new StructuredMessage<T>
            {
                AuthorName = msg.AuthorName ?? string.Empty,
                CreatedAt = msg.CreatedAt?.DateTime ?? DateTime.UtcNow,
                Role = MapMessageRole(msg.Role.ToString()),
                MessageId = msg.MessageId ?? string.Empty,
                StructuredOutput = msg.Role.ToString().ToLowerInvariant() == "assistant" ? structuredOutput : default(T?)
            }).ToList() ?? new List<StructuredMessage<T>>()
        };

        if (agentThread != null)
        {
            response.ThreadId = ExtractThreadId(agentThread);
        }

        return response;
    }

    /// <summary>
    /// Maps the Microsoft.Agents.AI message role to the shared MessageRole enum.
    /// </summary>
    private AzureAIFoundryShared.Models.MessageRole MapMessageRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return AzureAIFoundryShared.Models.MessageRole.User;

        return role.ToLowerInvariant() switch
        {
            "assistant" => AzureAIFoundryShared.Models.MessageRole.Assistant,
            "system" => AzureAIFoundryShared.Models.MessageRole.System,
            "tool" => AzureAIFoundryShared.Models.MessageRole.Tool,
            "user" => AzureAIFoundryShared.Models.MessageRole.User,
            _ => AzureAIFoundryShared.Models.MessageRole.User
        };
    }

    /// <summary>
    /// Extracts the thread ID (conversationId) from an AgentThread.
    /// </summary>
    private string ExtractThreadId(AgentThread agentThread)
    {
        if (agentThread == null)
        {
            return string.Empty;
        }

        try
        {
            string serializedJson = agentThread.Serialize(JsonSerializerOptions.Web).GetRawText();
            JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(serializedJson, JsonSerializerOptions.Web);
            
            if (jsonElement.TryGetProperty("conversationId", out var conversationIdElement))
            {
                var threadId = conversationIdElement.GetString();
                if (!string.IsNullOrWhiteSpace(threadId))
                {
                    return threadId;
                }
            }
        }
        catch
        {
        }

        return string.Empty;
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

