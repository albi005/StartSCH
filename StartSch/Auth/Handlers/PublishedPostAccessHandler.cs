using Microsoft.AspNetCore.Authorization;
using StartSch.Auth.Requirements;
using StartSch.Data;

namespace StartSch.Auth.Handlers;

/// Allows reading a post if it has been published.
public class PublishedPostAccessHandler : AuthorizationHandler<ResourceAccessRequirement, Post>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceAccessRequirement requirement,
        Post post)
    {
        if (requirement.AccessLevel == AccessLevel.Read && post.PublishedUtc.HasValue)
            context.Succeed(requirement);
        return Task.CompletedTask;
    }
}