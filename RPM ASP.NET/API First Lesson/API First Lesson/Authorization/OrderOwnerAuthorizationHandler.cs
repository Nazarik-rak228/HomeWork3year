using API_First_Lesson.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API_First_Lesson.Authorization;

public class OrderOwnerAuthorizationHandler : AuthorizationHandler<OrderOwnerRequirement, Order>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OrderOwnerRequirement requirement,
        Order resource)
    {
        // Админ может всё
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
        {
            if (resource.UserId == userId)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}