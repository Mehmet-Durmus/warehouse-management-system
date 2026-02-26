using MediatR;

namespace WHMS.Application.Features.Command.Login;

public class LoginCommandRequest : IRequest<LoginCommandResponse>
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
}