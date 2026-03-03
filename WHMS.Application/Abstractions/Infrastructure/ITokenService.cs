using WHMS.Application.DTOs;
using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Infrastructure;

public interface ITokenService
{
    AccessTokenDto CreateAccessToken(ApplicationUser user, IList<string> roles, int lifeTimeMinutes);
}