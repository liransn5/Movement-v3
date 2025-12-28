using DataRetriever.Data.Repositories;

namespace DataRetriever.Services.Providers;

public class DbProvider : DataProviderBase<string, string>
{
    private readonly IDataRepository _repository;

    public DbProvider(IDataRepository repository)
    {
        _repository = repository;
    }

    public override async Task<(bool Found, string Value)> GetAsync(string key)
    {
        var item = await _repository.GetAsync(key);
        if (item != null)
            return (true, item.Value);

        if (_next != null)
        {
            var result = await _next.GetAsync(key);
            if (result.Found)
                return result;
        }

        return (false, default);
    }

    public override async Task SaveAsync(string key, string value)
    {
        await _repository.AddAsync(key, value);
    }
}

