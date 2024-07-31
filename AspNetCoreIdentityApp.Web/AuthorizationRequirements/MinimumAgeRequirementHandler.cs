using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.AuthorizationRequirements
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int Age { get; set; }
        public MinimumAgeRequirement(int age)
        {
            Age = age;
        }
    }

    public class MinimumAgeRequirementHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            //if the claim type is not DateOfBirth
            if(!context.User.HasClaim(x => x.Type == ClaimTypes.DateOfBirth))
            {
                return Task.CompletedTask;
            }

            var birthDateClaim = context.User.FindFirst(x => x.Type == ClaimTypes.DateOfBirth).Value;

            DateTime birthDate = DateTime.Parse(birthDateClaim);

            int age = GetAge(birthDate,DateTime.UtcNow);


            if (age >= requirement.Age)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
        public int GetAge(DateTime birthDate, DateTime now)
        {
            int age = now.Year - birthDate.Year;

            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
                age--;

            return age;
        }
    }


   
}
