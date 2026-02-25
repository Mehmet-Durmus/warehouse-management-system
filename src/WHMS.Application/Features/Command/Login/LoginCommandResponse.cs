namespace WHMS.Application.Features.Command.Login;

public class LoginCommandResponse
{
    public required string AccessToken{ get; set; }
    public DateTime Expiration { get; set; }
}