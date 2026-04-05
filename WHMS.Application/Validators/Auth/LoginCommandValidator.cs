using FluentValidation;
using WHMS.Application.Features.Command.Auth.Login;

namespace WHMS.Application.Validators.Auth;

public class LoginCommandValidator : AbstractValidator<LoginCommandRequest>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Provide a user name.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Provide a password.");
    }
}