namespace WHMS.Application.Features.Command.Auth.Login;

public class LoginCommandResponse
{
    public required string AccessToken{ get; set; }
    public DateTime Expiration { get; set; }
}