using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.AuthorizationRequirements
{
    public class PageExpireRequirement : IAuthorizationRequirement
    {
    }

    public class PageExpireRequirementHandler : AuthorizationHandler<PageExpireRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PageExpireRequirement requirement)
        {
            //if the claim type is not DateOfBirth
            if (!context.User.HasClaim(x => x.Type == "PageExpireDate"))
            {
                return Task.CompletedTask;
            }

            var birthDateClaim = context.User.FindFirst(x => x.Type == "PageExpireDate").Value;

            DateTime birthDate = DateTime.Parse(birthDateClaim);


            return Task.CompletedTask;
        }
    }
}
