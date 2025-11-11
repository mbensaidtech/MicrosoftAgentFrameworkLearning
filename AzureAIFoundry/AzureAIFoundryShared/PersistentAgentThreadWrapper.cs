using Microsoft.Agents.AI;
using Azure.AI.Agents.Persistent;

namespace AzureAIFoundryShared;

/// <summary>
/// Wrapper class that implements AgentThread and wraps PersistentAgentThread.
/// This allows us to use PersistentAgentThread with RunAsync which expects AgentThread.
/// </summary>
public class PersistentAgentThreadWrapper : AgentThread
{
    private readonly PersistentAgentThread _persistentThread;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentAgentThreadWrapper"/> class.
    /// </summary>
    /// <param name="persistentThread">The PersistentAgentThread to wrap.</param>
    public PersistentAgentThreadWrapper(PersistentAgentThread persistentThread)
    {
        _persistentThread = persistentThread ?? throw new ArgumentNullException(nameof(persistentThread));
    }

    /// <summary>
    /// Gets the ID of the thread.
    /// </summary>
    public string Id => _persistentThread.Id;

    /// <summary>
    /// Gets the wrapped PersistentAgentThread.
    /// </summary>
    public PersistentAgentThread PersistentThread => _persistentThread;
}

