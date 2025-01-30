using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace mutraw_marketplace_api.Utilities;

public class OwnerOrAdminHandler(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<OwnerOrAdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
        OwnerOrAdminRequirement requirement)
    {
        // Check if the user is authenticated
        if(context.User.Identity is { IsAuthenticated: false })
        {
            return Task.CompletedTask;
        }
        
        // Check if the user is an Admin
        if(context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var routeData = httpContextAccessor.HttpContext?.Request.RouteValues;


        if (routeData is null || !routeData.TryGetValue("userId", out var value)) return Task.CompletedTask;
        var userIdFromRoute = value?.ToString();
        if (userIdFromRoute == userIdClaim)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}