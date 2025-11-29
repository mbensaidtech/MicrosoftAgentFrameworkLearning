using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using AzureAIFoundryShared;
using Microsoft.Extensions.Configuration;
using A2A;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using AzureAIFoundryA2AServer;
using Microsoft.AspNetCore.Builder;
using A2A.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Load AgentConfiguration
var agentConfig = new AgentConfig();
configuration.GetSection("AgentConfiguration").Bind(agentConfig);

// Instantiate PersistentAgentsClientFacade
var facade = new PersistentAgentsClientFacade(agentConfig);

var hostedAgentService = new HostedAgentService(facade);

// Call GetOrCreateAgentAsync
AIAgent hostedA2AAgent;
AgentCard hostedA2ACard;
(hostedA2AAgent, hostedA2ACard) = await hostedAgentService.GetHostedAgentAsync("asst_HLA6TXPwHNpmJcMawbzOOHOE");

Console.WriteLine($"Agent created/retrieved: {hostedA2AAgent.Name}");

app.MapA2A(
    hostedA2AAgent,
    path: "/",
    agentCard: hostedA2ACard,
    taskManager => app.MapWellKnownAgentCard(taskManager, "/"));
 await app.RunAsync();

 return;
