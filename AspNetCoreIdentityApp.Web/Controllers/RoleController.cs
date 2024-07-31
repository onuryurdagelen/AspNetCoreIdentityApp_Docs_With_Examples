using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RoleController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var roleViewModels = roles.Select(x => new RoleViewModel {RoleId = x.Id, Name= x.Name }).ToList();

            return View(roleViewModels);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(AddRoleViewModel request)
        {

            if (ModelState.IsValid) {

                IdentityResult result = await _roleManager.CreateAsync(new AppRole { Name = request.Name });

                if (!result.Succeeded)
                {
                    ModelState.AddModelStateError(result.Errors.ToArray());
                    return View();
                }
                TempData["SuccessMessage"] = "Role successfully added!";
                return RedirectToAction(nameof(HomeController.Index));
            }
            return View();
           
        }
    }
}
