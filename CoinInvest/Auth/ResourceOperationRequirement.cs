using Microsoft.AspNetCore.Authorization;

namespace CoinInvest.Auth
{
    public enum ResourceOperation
    {
        Create,
        Read,
        Update,
        Delete
    }
    public class ResourceOperationRequirement : IAuthorizationRequirement
    {
        public ResourceOperationRequirement(ResourceOperation  resourceOperation)
        {
            ResourceOperation = ResourceOperation;
        }
        public ResourceOperation  ResourceOperation { get; }
    }
}
