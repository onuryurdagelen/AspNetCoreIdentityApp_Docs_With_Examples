using AspNetCoreIdentityApp.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Rewrite;

namespace AspNetCoreIdentityApp.Web.Areas.Client.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public bool CheckIdentityResults(IdentityResult result,ModelStateDictionary modelState)
        {
            if (!result.Succeeded)
            {
                modelState.AddModelStateError(result.Errors.ToArray());
                //modelState.AddModelStateError(new IdentityError[] { new() { Code = string.Empty, Description = "Something went wrong!" } });
                return false;
            }
            return true;
        }
    }
}
