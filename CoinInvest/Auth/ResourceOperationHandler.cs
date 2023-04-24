using CoinInvest.DataAccessLayer.Entity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CoinInvest.Auth
{
    public class ResourceOperationHandler : AuthorizationHandler<ResourceOperationRequirement, Coin>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement, Coin resource)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(resource.UserId == Int32.Parse(userId))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
