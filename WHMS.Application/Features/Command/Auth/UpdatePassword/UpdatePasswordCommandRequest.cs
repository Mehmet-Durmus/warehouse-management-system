using MediatR;

namespace WHMS.Application.Features.Command.Auth.UpdatePassword;

public class UpdatePasswordCommandRequest : IRequest<UpdatePasswordCommandResponse>
{
    public string? CurrentPassword { get; set; }
    public string? Password { get; set; }
    public string? PasswordConfirm { get; set; }
}