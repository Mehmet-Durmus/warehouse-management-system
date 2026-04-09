namespace WHMS.Application.Common.DTOs;

public class AccessTokenDto
{
    public required string AccessToken { get; set; }
    public DateTime Expiration { get; set; }
}