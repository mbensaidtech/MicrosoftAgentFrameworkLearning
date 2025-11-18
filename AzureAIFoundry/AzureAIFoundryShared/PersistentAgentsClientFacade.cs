using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using Azure.Identity;
using Microsoft.Agents.AI;
using Azure.AI.Agents.Persistent;
using Azure;
using static CommonUtilities.ColoredConsole;
using System.Linq;

namespace AzureAIFoundryShared;

/// <summary>
/// Facade for persistent agents client operations.
/// Provides a unified interface for managing agents, vector stores, and files.
/// </summary>
public partial class PersistentAgentsClientFacade : IPersistentAgentsClientFacade
{
    private readonly AgentConfig _agentConfig;
    private readonly PersistentAgentsClient _persistentAgentsClient;
    private readonly PersistentAgentsAdministrationClient _persistentAgentsAdministrationClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentAgentsClientFacade"/> class.
    /// </summary>
    /// <param name="agentConfig">The agent configuration.</param>
    public PersistentAgentsClientFacade(AgentConfig agentConfig)
    {
        _agentConfig = agentConfig ?? throw new ArgumentNullException(nameof(agentConfig));

        var credential = new DefaultAzureCredential(
            new DefaultAzureCredentialOptions { ExcludeAzureDeveloperCliCredential = false });

        WriteSecondaryLogLine($"Endpoint: {_agentConfig.GetEndpoint()}");
        _persistentAgentsClient = new(_agentConfig.GetEndpoint(), credential);
        _persistentAgentsAdministrationClient = _persistentAgentsClient.Administration;
    }

}

