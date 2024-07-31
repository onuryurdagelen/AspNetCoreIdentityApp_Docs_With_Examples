using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class ModelStateExtension
    {

        public static void AddModelStateError(this ModelStateDictionary modelState, IdentityError[] errors)
        {
            foreach (var error in errors) 
            {
                modelState.AddModelError(error.Code, error.Description);
            }

        }
    }
}
