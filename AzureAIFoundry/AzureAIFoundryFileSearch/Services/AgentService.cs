using AgentConfiguration;
using AzureAIFoundryShared;
using AzureAIFoundryFileSearch.Models;
using Microsoft.Agents.AI;

namespace AzureAIFoundryFileSearch.Services;

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
        var agent = await _persistentAgentsClientFacade.GetOrCreateAgentAsync(null, agentType);
        
        return new AgentInfo
        {
            AgentId = agent.Id
        };
    }
}

