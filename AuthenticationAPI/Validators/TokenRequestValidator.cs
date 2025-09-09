using AuthenticationAPI.Models.Requests;
using FluentValidation;

namespace AuthenticationAPI.Validators;

public class TokenRequestValidator :  AbstractValidator<TokenRequest>
{
    public TokenRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}