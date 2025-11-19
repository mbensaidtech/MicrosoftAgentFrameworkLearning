using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using Azure.AI.OpenAI;  
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.OpenAI;
using OpenAI.Chat;
using OpenAI;

namespace AzureOpenAIShared;

/// <summary>
/// Factory for creating OpenAI agents using AzureOpenAIClient.
/// </summary>
public class OpenAIAgentFactory : IOpenAIAgentFactory
{
    private readonly ChatClient _chatClient;
    private readonly AgentConfig _agentConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIAgentFactory"/> class.
    /// </summary>
    /// <param name="agentConfig">The agent configuration containing endpoint and deployment information.</param>
    public OpenAIAgentFactory(AgentConfig agentConfig)
    {
        _agentConfig = agentConfig ?? throw new ArgumentNullException(nameof(agentConfig));

        var credential = new DefaultAzureCredential(
            new DefaultAzureCredentialOptions { ExcludeAzureDeveloperCliCredential = false });

        var endpoint = new Uri(_agentConfig.GetEndpoint());
        var azureOpenAIClient = new AzureOpenAIClient(endpoint, credential);
        
        // Create the ChatClient once in the constructor since it will always be the same
        var deploymentName = _agentConfig.GetDeploymentName();
        _chatClient = azureOpenAIClient.GetChatClient(deploymentName);
    }

    /// <summary>
    /// Creates a basic AIAgent with instructions and name based on the specified agent type.
    /// </summary>
    /// <param name="agentType">The type of agent to create, which determines the configuration to use.</param>
    /// <returns>The created AIAgent.</returns>
    public AIAgent CreateBasicAIAgent(AgentType agentType)
    {
        var agentSettings = _agentConfig.GetAgent(agentType);
        
        var agent = _chatClient.CreateAIAgent(
            instructions: agentSettings.Instructions,
            name: agentSettings.Name);
        
        return agent;
    }

    /// <summary>
    /// Creates an advanced AIAgent with instructions and name based on the specified agent type.
    /// </summary>
    /// <param name="agentType">The type of agent to create, which determines the configuration to use.</param>
    /// <returns>The created AIAgent.</returns>
    public AIAgent CreateAdvancedAIAgent(AgentType agentType)
    {
        var agentSettings = _agentConfig.GetAgent(agentType);
        
        var agent = _chatClient.CreateAIAgent(
            instructions: agentSettings.Instructions,
            name: agentSettings.Name);
        
        return agent;
    }
}

