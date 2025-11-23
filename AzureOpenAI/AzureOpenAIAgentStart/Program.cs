using AgentConfiguration;
using AzureOpenAIShared;
using AzureOpenAIAgentStart.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddAgentConfiguration(builder.Configuration);


builder.Services.AddOpenAIAgentFactory();


builder.Services.AddSingleton<IAgentService, AgentService>();

var app = builder.Build();


app.UseHttpsRedirection();

app.MapControllers();

app.Run();
