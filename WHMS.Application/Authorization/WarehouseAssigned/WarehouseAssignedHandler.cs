using Microsoft.AspNetCore.Authorization;
using WHMS.Application.Common.Constants;

namespace WHMS.Application.Authorization.WarehouseAssigned;

public class WarehouseAssignedHandler : AuthorizationHandler<WarehouseAssignedRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WarehouseAssignedRequirement requirement)
    {
        if (context.User.IsInRole(ApplicationRole.LogisticDirector))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var warehouseId = context.User.FindFirst("Warehouse")?.Value;

        if (!string.IsNullOrEmpty(warehouseId))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}