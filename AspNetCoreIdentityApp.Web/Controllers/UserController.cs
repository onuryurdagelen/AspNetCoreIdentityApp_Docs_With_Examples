using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspNetCoreIdentityApp.Service.Services;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMemberService _memberService;

        public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IMemberService memberService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _memberService = memberService;
        }

        public async Task<IActionResult> Index()
        {
           var result = await _memberService.GetUserViewModelList();

            return View(result);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);

            var allRoles =  await _roleManager.Roles.ToListAsync();

            var rolesByUser = await _userManager.GetRolesAsync(user!);

            return View(new UserEditViewModel 
            { 
                EmailAddress = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.PhoneNumber,
                UserId = user.Id,
                RolesList = allRoles.ToList().Select(x => new SelectListItem
                {
                    Value = x.Name,
                    Text = x.Name
                }),
                SelectedRoles = rolesByUser.ToList()
            });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel request)
        {

            var user = await _userManager.FindByIdAsync(request.UserId);

            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            if (request.SelectedRoles.Any()) 
            {

                foreach (var role in roles)
                {
                    if (userRoles.Contains(role.Name))
                    {
                        await _userManager.RemoveFromRoleAsync(user!, role.Name);
                    }

                }
                await _userManager.AddToRolesAsync(user!, request.SelectedRoles);
            }
            user.PhoneNumber = request.Phone;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.EmailAddress;

            IdentityResult result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelStateError(result.Errors.ToArray());
                return View(request);
            }
            await _userManager.UpdateSecurityStampAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}
