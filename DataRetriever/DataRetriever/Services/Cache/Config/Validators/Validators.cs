namespace DataRetriever.Services.Cache.Config.Validators;

public interface IConfigValidator<T>
{
    void Validate(T config);
}

public abstract class ConfigValidatorBase<T> : IConfigValidator<T>
{
    private readonly IReadOnlyCollection<IValidationRule<T>> _rules;

    protected ConfigValidatorBase(IEnumerable<IValidationRule<T>> rules)
    {
        _rules = rules.ToList();        
    }

    /// <summary>
    /// Validates the given configuration instance against all registered validation rules.
    /// </summary>
    /// <param name="config">
    /// The configuration object to validate.
    /// </param>
    public void Validate(T config)
    {
        foreach (var rule in _rules)
            rule.Validate(config);
    }
}

public class InMemoryStorageConfigValidator : ConfigValidatorBase<InMemoryStorageConfig>
{
    public InMemoryStorageConfigValidator() : base(new IValidationRule<InMemoryStorageConfig>[]
        {new CapacityRangeRule()})
    {
    }
}
