using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgentConfiguration;

/// <summary>
/// Extension methods for configuring agent services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// The name of the configuration section for agent configuration.
    /// </summary>
    public const string ConfigurationSectionName = "AgentConfiguration";

    /// <summary>
    /// Adds the AgentConfiguration to the service collection and binds it from the configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration section does not exist or is empty.</exception>
    public static IServiceCollection AddAgentConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var configurationSection = configuration.GetSection(ConfigurationSectionName);

        if (!configurationSection.Exists() || !configurationSection.GetChildren().Any())
        {
            throw new InvalidOperationException(
                $"The configuration section '{ConfigurationSectionName}' is missing or empty. " +
                $"Please ensure it exists in your configuration file.");
        }

        services.Configure<AgentConfiguration>(configurationSection);

        services.AddSingleton<AgentConfiguration>(sp =>
        {
            var config = new AgentConfiguration();
            configurationSection.Bind(config);
            return config;
        });

        return services;
    }
}

