using AgentConfiguration;
using AzureAIFoundryShared;
using AzureAIFoundryStart.Models;
using Microsoft.Agents.AI;

namespace AzureAIFoundryStart.Services;

/// <summary>
/// Service for agent management.
/// </summary>
public class AgentService : IAgentService
{
    private readonly IPersistentAgentsClientFacade _persistentAgentsClientFacade;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentService"/> class.
    /// </summary>
    /// <param name="persistentAgentsClientFacade">The persistent agents client facade.</param>
    public AgentService(IPersistentAgentsClientFacade persistentAgentsClientFacade)
    {
        _persistentAgentsClientFacade = persistentAgentsClientFacade ?? throw new ArgumentNullException(nameof(persistentAgentsClientFacade));
    }

    /// <inheritdoc/>
    public async Task<AgentInfo> CreateAgentAsync(AgentType agentType)
    {
        // Pass null for agentId to ensure a new agent is always created
        var agent = await _persistentAgentsClientFacade.GetOrCreateAgentAsync(null, agentType);

        return new AgentInfo
        {
            AgentId = agent.Id
        };
    }
}

