using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();


            bool isNumberic = int.TryParse(user.UserName[0].ToString(),out _);

            if (isNumberic) 
            {
                errors.Add(new() { Code = "UsernameNumberic", Description = "Username cannot start with a numeric character" });
            }
            if (!errors.Any()) 
                return Task.FromResult(IdentityResult.Success);

            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
                       
        }
    }
}
