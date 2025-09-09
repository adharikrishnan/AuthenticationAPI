using FluentValidation;
using FluentValidation.Results;

namespace AuthenticationAPI.Validators;

public class ValidatorFactory(IServiceProvider serviceProvider) : IValidatorFactory
{
    public IValidator<T> GetValidator<T>()
    {
        var validator = serviceProvider.GetService<IValidator<T>>();
        return validator
               ?? throw new NullReferenceException("Specified Validator could not be found.");
    }

    public ValidationResult HandleValidation<T>(T instance)
    {
        var validator = serviceProvider.GetService<IValidator<T>>();
        return validator!.Validate(instance);
    }

    public async Task<ValidationResult> HandleValidationAsync<T>(T instance, CancellationToken ct)
    {
        var validator = serviceProvider.GetService<IValidator<T>>();
        return await validator!.ValidateAsync(instance, ct).ConfigureAwait(false);
    }
}