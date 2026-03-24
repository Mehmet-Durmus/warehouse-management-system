using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WHMS.Application.Abstractions.Infrastructure;

namespace WHMS.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?
                .User.FindFirstValue(ClaimTypes.NameIdentifier);
                return claim != null ? Guid.Parse(claim) : null;
        }
    }

    public string? FullName
    {
        get => _httpContextAccessor.HttpContext?
            .User.FindFirstValue(ClaimTypes.Name);
    }

    public string? UserName
    {
        get => _httpContextAccessor.HttpContext?
            .User.FindFirstValue("UserName");
    }

    public string? WarehouseId 
    { 
        get => _httpContextAccessor.HttpContext?
            .User.FindFirstValue("Warehouse");
    }
    
}