using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using AzureAIFoundryShared;
using A2A;

namespace AzureAIFoundryA2AServer;

public class HostedAgentService
{
    private readonly IPersistentAgentsClientFacade _persistentAgentsClientFacade;

    public HostedAgentService(IPersistentAgentsClientFacade persistentAgentsClientFacade)
    {
        _persistentAgentsClientFacade = persistentAgentsClientFacade;
    }

    public async Task<(AIAgent,AgentCard)> GetHostedAgentAsync(string agentId)
    {
        var agent = await _persistentAgentsClientFacade.GetOrCreateAgentAsync(agentId, null);
        var agentCard = AgentCards.CreateGlobalAgentCard();
        return new (agent, agentCard);
    }
}