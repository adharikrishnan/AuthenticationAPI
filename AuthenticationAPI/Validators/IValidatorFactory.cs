using FluentValidation;
using FluentValidation.Results;

namespace AuthenticationAPI.Validators;

public interface IValidatorFactory
{
    IValidator<T> GetValidator<T>();

    ValidationResult HandleValidation<T>(T instance);
    Task<ValidationResult> HandleValidationAsync<T>(T instance, CancellationToken ct);
}