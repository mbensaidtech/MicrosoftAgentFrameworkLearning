using AgentConfiguration;
using AzureOpenAIShared.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AzureOpenAIShared;

/// <summary>
/// Extension methods for configuring Azure OpenAI services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// The name of the configuration section for vector store configuration.
    /// </summary>
    public const string VectorStoreConfigurationSectionName = "VectorStore";

    /// <summary>
    /// Adds the VectorStoreConfiguration to the service collection and binds it from the configuration.
    /// Also registers MongoDB services (MongoClient and IMongoDatabase) as singletons if MongoDB configuration exists.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddVectorStoreConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var configurationSection = configuration.GetSection(VectorStoreConfigurationSectionName);

        // Configure IOptions pattern
        services.Configure<VectorStoreConfiguration>(configurationSection);

        // Register as singleton for direct access (similar to AgentConfiguration pattern)
        services.AddSingleton<VectorStoreConfiguration>(sp =>
        {
            var config = new VectorStoreConfiguration();
            configurationSection.Bind(config);
            return config;
        });

        // Check if MongoDB configuration exists
        var mongoDbSection = configurationSection.GetSection("MongoDb");
        if (mongoDbSection.Exists() && 
            !string.IsNullOrWhiteSpace(mongoDbSection["ConnectionString"]) &&
            !string.IsNullOrWhiteSpace(mongoDbSection["DatabaseName"]))
        {
            // Register MongoDB services only if configuration is present
            services.AddSingleton<MongoClient>(sp =>
            {
                var config = sp.GetRequiredService<IOptions<VectorStoreConfiguration>>().Value;
                if (string.IsNullOrWhiteSpace(config.MongoDb.ConnectionString))
                {
                    throw new InvalidOperationException("MongoDB ConnectionString is required but was not found in configuration.");
                }
                return new MongoClient(config.MongoDb.ConnectionString);
            });

            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var mongoClient = sp.GetRequiredService<MongoClient>();
                var config = sp.GetRequiredService<IOptions<VectorStoreConfiguration>>().Value;
                if (string.IsNullOrWhiteSpace(config.MongoDb.DatabaseName))
                {
                    throw new InvalidOperationException("MongoDB DatabaseName is required but was not found in configuration.");
                }
                return mongoClient.GetDatabase(config.MongoDb.DatabaseName);
            });
        }
        else
        {
            // Register IMongoDatabase as null when MongoDB is not configured
            // This allows the simple DI registration format to work
            services.AddSingleton<IMongoDatabase>(sp => null!);
        }

        return services;
    }

    /// <summary>
    /// Adds the OpenAI agent factory to the service collection as a singleton.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenAIAgentFactory(this IServiceCollection services)
    {
        services.AddSingleton<IOpenAIAgentFactory, OpenAIAgentFactory>();
        return services;
    }
}

