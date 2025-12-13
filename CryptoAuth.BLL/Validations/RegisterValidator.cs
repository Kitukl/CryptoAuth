using CryptoAuth.BLL.DTOs;
using FluentValidation;

namespace CryptoAuth.BLL.Validations;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .MinimumLength(20);
        RuleFor(r => r.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}