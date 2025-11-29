using A2A;
namespace AzureAIFoundryA2AServer;

/// <summary>
/// Contains static methods for creating agent cards for each agent type.
/// </summary>
public static class AgentCards
{
    public static AgentCard CreateGlobalAgentCard()
    {
        var getRandomNumber = new AgentSkill() {
            Id="global_agent_get_random_number",
            Name = "GetRandomNumber",
            Description = "Get a random number between 0 and 100.",
            Tags = ["random"],
            Examples = ["Get a random number between 0 and 100."],
        };

        return new AgentCard
        {
            Name = "GlobalAgent",
            Description = "A global agent that can answer questions about any topic.",
            Version = "1.0.0",
            DefaultInputModes = ["text"],
            DefaultOutputModes = ["text"],
            Capabilities = new AgentCapabilities() {

                Streaming = false,
                PushNotifications = false,
            },
            Skills = [getRandomNumber],
            Url="http://localhost:5000"
        };
    }

   
}

