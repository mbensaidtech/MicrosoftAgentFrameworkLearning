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

// Register persistent agents client facade as singleton (Azure SDK clients are thread-safe and stateless)
builder.Services.AddSingleton<IPersistentAgentsClientFacade, PersistentAgentsClientFacade>();

// Register agent service as singleton (stateless service)
builder.Services.AddSingleton<IAgentService, AgentService>();

// Register thread service as singleton (stateless service)
builder.Services.AddSingleton<IThreadService, ThreadService>();

// Register vector store service as singleton (stateless service)
builder.Services.AddSingleton<IVectorStoreService, VectorStoreService>();

// Register dataset service as singleton (stateless service)
builder.Services.AddSingleton<IDatasetService, DatasetService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
