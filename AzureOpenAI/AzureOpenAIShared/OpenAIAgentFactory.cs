using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using Azure.AI.OpenAI;  
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.OpenAI;
using OpenAI.Chat;
using OpenAI;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Connectors.MongoDB;
using Microsoft.Extensions.VectorData;
using Microsoft.Extensions.AI;
using AzureOpenAIShared.Stores;
using MongoDB.Driver;
using AzureOpenAIShared.Middleware;

namespace AzureOpenAIShared;

/// <summary>
/// Factory for creating OpenAI agents using AzureOpenAIClient.
/// </summary>
public class OpenAIAgentFactory : IOpenAIAgentFactory
{
    private readonly ChatClient _chatClient;
    private readonly AgentConfig _agentConfig;
    private readonly IMongoDatabase? _mongoDatabase;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIAgentFactory"/> class.
    /// </summary>
    /// <param name="agentConfig">The agent configuration containing endpoint and deployment information.</param>
    /// <param name="mongoDatabase">Optional MongoDB database instance for vector store operations.</param>
    public OpenAIAgentFactory(AgentConfig agentConfig, IMongoDatabase? mongoDatabase = null)
    {
        _agentConfig = agentConfig ?? throw new ArgumentNullException(nameof(agentConfig));
        _mongoDatabase = mongoDatabase;

        var credential = new DefaultAzureCredential(
            new DefaultAzureCredentialOptions { ExcludeAzureDeveloperCliCredential = false });

        var endpoint = new Uri(_agentConfig.GetEndpoint());
        var azureOpenAIClient = new AzureOpenAIClient(endpoint, credential);
        
        var deploymentName = _agentConfig.GetDeploymentName();
        _chatClient = azureOpenAIClient.GetChatClient(deploymentName);
    }

    /// <summary>
    /// Creates a basic AIAgent.
    /// </summary>
    /// <returns>The created AIAgent.</returns>
    public AIAgent CreateBasicAIAgent()
    {  
        return _chatClient.CreateAIAgent();
    }

    /// <summary>
    /// Creates an advanced AIAgent with instructions and name based on the specified agent type.
    /// Optionally includes tools and applies function call middleware.
    /// </summary>
    /// <param name="request">The request containing agent type and optional tools.</param>
    /// <returns>The created AIAgent.</returns>
    public AIAgent CreateAdvancedAIAgent(CreateAdvancedAIAgentRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var agentSettings = _agentConfig.GetAgent(request.AgentType);
        
        if (request.Tools != null && request.Tools.Count > 0)
        {
            var agentBuilder = _chatClient.CreateAIAgent(
                instructions: agentSettings.Instructions,
                name: agentSettings.Name,
                tools: request.Tools)
                .AsBuilder();

            if (request.EnableFunctionCallMiddleware)
            {
                agentBuilder = agentBuilder.Use(FunctionCallMiddleware.OnFunctionCall);
            }

            return agentBuilder.Build();
        }
        else
        {
            var agent = _chatClient.CreateAIAgent(
                instructions: agentSettings.Instructions,
                name: agentSettings.Name);
            return agent;
        }
    }

    public AIAgent CreateAgentWithVectorStore(AgentType agentType, VectorStoresTypes vectorStoreType)
    {
        var agentSettings = _agentConfig.GetAgent(agentType);
        VectorStore vectorStore = vectorStoreType switch
        {
            VectorStoresTypes.InMemory => new InMemoryVectorStore(),
            VectorStoresTypes.Mongo => _mongoDatabase == null 
                ? throw new InvalidOperationException("MongoDB database is required for Mongo vector store type but was not provided.")
                : new MongoVectorStore(_mongoDatabase),
            _ => throw new ArgumentException($"Unknown vector store type: {vectorStoreType}", nameof(vectorStoreType))
        };
        
        var orchestratorOptions = new ChatClientAgentOptions
        {
            Instructions = agentSettings.Instructions,
            Name = agentSettings.Name,
            ChatMessageStoreFactory = ctx =>
            {
                return new Stores.VectorChatMessageStore(
                    vectorStore,
                    ctx.SerializedState,
                    ctx.JsonSerializerOptions);
            }
        };
        var agent = _chatClient.CreateAIAgent(orchestratorOptions);
        
        return agent;
    }
}

