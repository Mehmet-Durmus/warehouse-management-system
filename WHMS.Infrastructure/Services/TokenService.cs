using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Common.Configurations;
using WHMS.Application.Common.DTOs;
using WHMS.Domain.Entities;

namespace WHMS.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public AccessTokenDto CreateAccessToken(ApplicationUser user, IList<string> roles, int lifeTimeMinutes)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim("UserName", user.UserName ?? String.Empty),
            new Claim("Warehouse", user.WarehouseId.ToString() ?? String.Empty),
            new Claim("IsPasswordChanged", user.IsPasswordChanged.ToString()),
            new Claim("PasswordChangedAt", user.PasswordChangedAt.ToString("O"))
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenDto
        {
            AccessToken = jwt,
            Expiration = expires
        };
    }
}