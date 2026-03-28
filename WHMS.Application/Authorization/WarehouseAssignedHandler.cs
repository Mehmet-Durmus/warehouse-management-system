using Microsoft.AspNetCore.Authorization;

namespace WHMS.Application.Authorization;

public class WarehouseAssignedHandler : AuthorizationHandler<WarehouseAssignedRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WarehouseAssignedRequirement requirement)
    {
        if (context.User.IsInRole("LogisticDirector"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var warehouseId = context.User.FindFirst("WarehouseId")?.Value;

        if (!string.IsNullOrEmpty(warehouseId))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}