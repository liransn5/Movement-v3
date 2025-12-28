namespace DataRetriever.Services.Cache.Config.Validators;

public interface IValidationRule<T>
{
    void Validate(T value);
}

public class CapacityRangeRule : IValidationRule<InMemoryStorageConfig>
{
    private const int MIN_CAPACITY = 3;
    private const int MAX_CAPACITY = 100;

    public void Validate(InMemoryStorageConfig config)
    {
        if (config.Capacity < MIN_CAPACITY || config.Capacity > MAX_CAPACITY)
        {
            throw new ArgumentOutOfRangeException(
                nameof(config.Capacity), $"Capacity must be between {MIN_CAPACITY} and {MAX_CAPACITY}");
        }
    }
}

