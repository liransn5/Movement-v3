namespace DataRetriever.Services.Cache.Config;

public class RedisStorageConfig
{
    public int TtlMinutes { get; set; }
    public string KeyPrefix { get; set; }
}