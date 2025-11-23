using AgentConfiguration;
using AzureOpenAIShared;
using AzureOpenAIAgentWithThreads.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add agent configuration from appsettings.json
builder.Services.AddAgentConfiguration(builder.Configuration);

// Add vector store configuration and register MongoDB services
builder.Services.AddVectorStoreConfiguration(builder.Configuration);

// Register OpenAI agent factory as singleton
builder.Services.AddOpenAIAgentFactory();

// Register agent service as singleton (agent will be created inside the service)
builder.Services.AddSingleton<IAgentService, AgentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
