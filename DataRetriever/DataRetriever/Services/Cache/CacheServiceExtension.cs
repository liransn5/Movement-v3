using DataRetriever.Services.Cache.CacheStorages;
using DataRetriever.Services.Cache.Config.Validators;
using DataRetriever.Services.Cache.Config;
using DataRetriever.Services.Cache.EvictionPolicies;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DataRetriever.Services.Cache;

public static class CacheServiceCollectionExtensions
{
    public static IServiceCollection AddSdscCache(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<InMemoryStorageConfig>(configuration.GetSection("InMemoryStorageConfig"));
        services.AddSingleton<IConfigValidator<InMemoryStorageConfig>, InMemoryStorageConfigValidator>();

        services.AddSingleton<ISdcsCache<string, string>>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<InMemoryStorageConfig>>().Value;
            var validator = sp.GetRequiredService<IConfigValidator<InMemoryStorageConfig>>();

            return new SdcsCache<string, string>(
                new InMemoryStorage<string, string>(config, validator),
                new LruEvictionPolicy<string>());
        });

        return services;
    }

    public static IServiceCollection AddRedisCache(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<RedisStorageConfig>(configuration.GetSection("RedisStorageConfig"));

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            return ConnectionMultiplexer.Connect(redisConnectionString);
        });

        services.AddSingleton<IRedisCache<string, string>>(sp =>
        {
            var cfg = sp.GetRequiredService<IOptions<RedisStorageConfig>>().Value;
            var redis = sp.GetRequiredService<IConnectionMultiplexer>();

            return new RedisCache<string, string>(
                new RedisStorage<string, string>(redis, cfg),
                new NoEvictionPolicy<string>());
        });

        return services;
    }
}
