using FluentValidation;
using WHMS.Application.Features.Command.Auth.UpdatePassword;

namespace WHMS.Application.Validators.Auth;

public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommandRequest>
{
    public UpdatePasswordCommandValidator()
    {
        RuleFor(x => x.PasswordConfirm)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");

        RuleFor(x => x.Password)
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("New password must be different from the current password.");
    }
}