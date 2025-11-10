using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure.Identity;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using OpenAI;
using Microsoft.Agents.AI;
using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using static CommonUtilities.ColoredConsole;

// Build configuration
var rootConfiguration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Set up dependency injection
var services = new ServiceCollection();
services.AddAgentConfiguration(rootConfiguration);
var serviceProvider = services.BuildServiceProvider();

// Get agent configuration from DI
var agentConfig = serviceProvider.GetRequiredService<AgentConfig>();

// Initialize Azure OpenAI client
var credential = new DefaultAzureCredential(
    new DefaultAzureCredentialOptions { ExcludeAzureDeveloperCliCredential = false });
var openAIClient = new AzureOpenAIClient(agentConfig.GetEndpointUri(), credential);
var chatClient = openAIClient.GetChatClient(agentConfig.GetDeploymentName());

// Get agent settings and create agent
var geographyAgentSettings = agentConfig.GetAgent("GeographyAgent");
var agent = chatClient.CreateAIAgent(new ChatClientAgentOptions
{
    Name = geographyAgentSettings.Name,
    Instructions = geographyAgentSettings.Instructions
});

WriteSystemLine("Ask me a geography question (or press 'x' to exit):");

while (true)
{
    Console.Write("> ");
    string? question = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(question))
    {
        continue;
    }
    
    if (question.Trim().ToLower() == "x")
    {
        WriteSystemLine("Goodbye!");
        break;
    }
    
    AgentRunResponse response = await agent.RunAsync(question);
    WriteAssistantLine(response?.ToString() ?? string.Empty);
    Console.WriteLine();
}