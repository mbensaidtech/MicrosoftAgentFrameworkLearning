using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using AzureAIFoundryShared;
using AzureAIFoundryFileSearch.Services;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add agent configuration from appsettings.json
builder.Services.AddAgentConfiguration(builder.Configuration);

// Register persistent agents client facade
builder.Services.AddScoped<IPersistentAgentsClientFacade, PersistentAgentsClientFacade>();

// Register agent service
builder.Services.AddScoped<IAgentService, AgentService>();

// Register thread service
builder.Services.AddScoped<IThreadService, ThreadService>();

// Register vector store service
builder.Services.AddScoped<IVectorStoreService, VectorStoreService>();

// Register dataset service
builder.Services.AddScoped<IDatasetService, DatasetService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
