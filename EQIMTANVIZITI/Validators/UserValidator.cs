using FluentValidation;
using EQIMTANVIZITI.DTOs;

namespace EQIMTANVIZITI.Validators;

public class UserValidator : AbstractValidator<RegisterRequest>
{
    public UserValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("type correct email");

        RuleFor(u => u.PhoneNumber)
            .NotEmpty()
            .WithMessage("write correct number");

        RuleFor(u => u.Password)
            .NotEmpty();
    }
}
