using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Azure.AI.OpenAI;
using OpenAI;
using OpenAI.Chat;
using Microsoft.Agents.AI;
using static CommonUtilities.ColoredConsole;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var agentConfig = configuration.GetSection("AgentConfiguration");

string deploymentVarName = agentConfig["DeploymentName"] 
    ?? throw new ArgumentNullException(nameof(deploymentVarName), "DeploymentName is not set in configuration");

string endpointVarName = agentConfig["Endpoint"] 
    ?? throw new ArgumentNullException(nameof(endpointVarName), "Endpoint is not set in configuration");

string deploymentName = Environment.GetEnvironmentVariable(deploymentVarName)
    ?? throw new ArgumentNullException(nameof(deploymentName), $"{deploymentVarName} environment variable is not set");

string endpoint = Environment.GetEnvironmentVariable(endpointVarName)
    ?? throw new ArgumentNullException(nameof(endpoint), $"{endpointVarName} environment variable is not set");

var authOptions = new DefaultAzureCredentialOptions { ExcludeAzureDeveloperCliCredential = false };

DefaultAzureCredential credential = new DefaultAzureCredential(authOptions);
AzureOpenAIClient openAIClient = new AzureOpenAIClient(new Uri(endpoint), credential);
OpenAI.Chat.ChatClient chatClient = openAIClient.GetChatClient(deploymentName);
AIAgent agent = chatClient.CreateAIAgent(new ChatClientAgentOptions()
{
    Name = "GeographyAgent",
    Instructions = "You are good at geography and you are a helpful assistant."
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