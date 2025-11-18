using AgentConfiguration;
using Microsoft.Agents.AI;
using Azure.AI.Agents.Persistent;
using Azure;

namespace AzureAIFoundryShared;

/// <summary>
/// Partial class for agent-related operations.
/// </summary>
public partial class PersistentAgentsClientFacade
{
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
    /// Gets a ChatClientAgent by its ID, or creates a new agent if the ID is not provided.
    /// </summary>
    /// <param name="agentId">The ID of the agent to retrieve. If null, a new agent will be created.</param>
    /// <param name="agentType">The type of agent to create. Required when agentId is null.</param>
    /// <returns>The ChatClientAgent.</returns>
    public async Task<ChatClientAgent> GetOrCreateChatClientAgentAsync(string? agentId, AgentConfiguration.AgentType? agentType)
    {
        AIAgent agent = await GetOrCreateAgentAsync(agentId, agentType);
        return (ChatClientAgent)agent;
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
}

