using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using AzureAIFoundryShared;
using AzureAIFoundryStart.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add agent configuration from appsettings.json
builder.Services.AddAgentConfiguration(builder.Configuration);

// Register persistent agents client facade
builder.Services.AddScoped<IPersistentAgentsClientFacade, PersistentAgentsClientFacade>();

// Register agent conversation service
builder.Services.AddScoped<IAgentConversationService, AgentConversationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
