using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {
            var errors = new List<IdentityError>();
            if(password!.ToLower().Contains(user.UserName!.ToLower()))
            {
                errors.Add(new() { Code = "PasswordContainsUserName",Description = "Password cannot contains username"});
            }
            if (password!.ToLower().StartsWith("1234"))
            {
                errors.Add(new() { Code = "PasswordContains1234",Description = "Password cannot contains consecutive numbers"});
            }
            if (!errors.Any())
                return Task.FromResult(IdentityResult.Success);

            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            
        }
    }
}
