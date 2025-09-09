using AuthenticationAPI.Models.Requests;
using FluentValidation;

namespace AuthenticationAPI.Validators;

public class AddUserValidator :  AbstractValidator<AddUserRequest>
{
    public AddUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
        
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be an empty string if specified.");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email address.");
        
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}