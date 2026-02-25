using MediatR;

namespace WHMS.Application.Features.Command.Login;

public class LoginCommandRequest : IRequest<LoginCommandResponse>
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}