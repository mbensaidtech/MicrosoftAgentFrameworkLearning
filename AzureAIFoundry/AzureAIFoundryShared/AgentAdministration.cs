using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using Azure.Identity;
using Microsoft.Agents.AI;
using Azure.AI.Agents.Persistent;
using Azure;

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

        Console.WriteLine($"Endpoint: {_agentConfig.GetEndpoint()}");
        _persistentAgentsClient = new(_agentConfig.GetEndpoint(), credential);
        _persistentAgentsAdministrationClient = _persistentAgentsClient.Administration;
    }

    /// <summary>
    /// Creates an agent with the specified parameters.
    /// </summary>
    /// <param name="request">The request containing agent creation parameters.</param>
    /// <returns>The created agent.</returns>
    public async Task<AIAgent> CreateAgentAsync(CreateAgentRequest request)
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

        Response<PersistentAgent> newPersistentAgent = _persistentAgentsAdministrationClient.CreateAgent(
            model: request.DeploymentName, 
            name: request.AgentName, 
            instructions: request.Instructions);
         return await _persistentAgentsClient.GetAIAgentAsync(newPersistentAgent.Value.Id);
    }

    /// <summary>
    /// Gets an agent by ID.
    /// </summary>
    /// <param name="agentId">The ID of the agent.</param>
    /// <returns>The agent.</returns>
    public async Task<AIAgent> GetAgentByIdAsync(string agentId)
    {
        if (string.IsNullOrWhiteSpace(agentId))
        {
            throw new ArgumentException("Agent ID cannot be null or empty.", nameof(agentId));
        }

        AIAgent agent = await _persistentAgentsClient.GetAIAgentAsync(agentId);
        return agent;
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
    /// Creates an agent request from the specified agent type.
    /// </summary>
    /// <param name="agentType">The type of agent to create a request for.</param>
    /// <returns>The agent creation request.</returns>
    public CreateAgentRequest CreateAgentRequest(AgentConfiguration.AgentType agentType)
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
    /// Deletes an agent by ID.
    /// </summary>
    /// <param name="agentId">The ID of the agent.</param>
    /// <returns>The task.</returns>
    public async Task DeleteAgentByIdAsync(string agentId)
    {
        if (string.IsNullOrWhiteSpace(agentId))
        {
            throw new ArgumentException("Agent ID cannot be null or empty.", nameof(agentId));
        }

        await _persistentAgentsAdministrationClient.DeleteAgentAsync(agentId);
    }

    /// <summary>
    /// Creates a thread.
    /// </summary>
    /// <returns>The created thread.</returns>
    public async Task<PersistentAgentThread> CreateThreadAsync()
    {
        PersistentAgentThread newPersistentThread = await _persistentAgentsClient.Threads.CreateThreadAsync();
        return newPersistentThread;
    }
    /// <summary>
    /// Gets a thread by ID.
    /// </summary>
    /// <param name="threadId">The ID of the thread.</param>
    /// <returns>The thread.</returns>
    public async Task<Response<PersistentAgentThread>> GetThreadByIdAsync(string threadId)
    {
        if (string.IsNullOrWhiteSpace(threadId))
        {
            throw new ArgumentException("Thread ID cannot be null or empty.", nameof(threadId));
        }

        Response<PersistentAgentThread> thread = await _persistentAgentsClient.Threads.GetThreadAsync(threadId);
        return thread;
    }

    /// <summary>
    /// Gets an AgentThread by ID for use with RunAsync.
    /// </summary>
    /// <param name="threadId">The ID of the thread.</param>
    /// <returns>The AgentThread.</returns>
    public async Task<AgentThread> GetAIThreadAsync(string threadId)
    {
        if (string.IsNullOrWhiteSpace(threadId))
        {
            throw new ArgumentException("Thread ID cannot be null or empty.", nameof(threadId));
        }

        // Get the PersistentAgentThread
        Response<PersistentAgentThread> threadResponse = await _persistentAgentsClient.Threads.GetThreadAsync(threadId);
        PersistentAgentThread persistentThread = threadResponse.Value;
        
        // Since PersistentAgentThread cannot be directly converted to AgentThread,
        // we wrap it in a PersistentAgentThreadWrapper that implements AgentThread
        return new PersistentAgentThreadWrapper(persistentThread);
    }

    /// <summary>
    /// Gets a PersistentAgentThread by ID.
    /// </summary>
    /// <param name="threadId">The ID of the thread.</param>
    /// <returns>The PersistentAgentThread.</returns>
    public async Task<PersistentAgentThread> GetPersistentAgentThreadAsync(string threadId)
    {
        if (string.IsNullOrWhiteSpace(threadId))
        {
            throw new ArgumentException("Thread ID cannot be null or empty.", nameof(threadId));
        }

        Response<PersistentAgentThread> threadResponse = await _persistentAgentsClient.Threads.GetThreadAsync(threadId);
        return threadResponse.Value;
    }
}

