using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager; 
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IFileProvider _fileProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, IStringLocalizer<HomeController> localizer, SignInManager<AppUser> signInManager, IFileProvider fileProvider, IWebHostEnvironment webHostEnvironment, RoleManager<AppRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _localizer = localizer;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            

            ViewData["Message"] = _localizer["Welcome"];
            return View();
            //return RedirectToAction("Register", "Home", new { area = "Client" });
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        

        public async Task<IActionResult> Roles()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]   
        public async Task<IActionResult> Profile()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            //get photo path from www root with IFileProvider

            var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

            var folderName = wwwrootFolder!.First(x => x.Name == "img").Name;

            var servicePathWithFolderName = Path.Combine($"{Request.Scheme}://{Request.Host}{Request.PathBase}/{folderName}/");

            var userViewModel = new ProfileEditViewModel { 
                UserId = currentUser!.Id,
                EmailAddress = currentUser!.Email!,
                FirstName = currentUser.FirstName,
                LastName = currentUser!.LastName,
                Phone = currentUser.PhoneNumber,
                City = currentUser.City,
                BirthDate = currentUser.BirthDate,
                GenderType = currentUser.Gender,
                PhotoSrc = currentUser!.Picture != null  ? Path.Combine(servicePathWithFolderName, currentUser!.Picture) : Path.Combine(servicePathWithFolderName, "default-user.webp"),
            };

            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileEditViewModel request)
        {

            if (ModelState.IsValid)
            {

                var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
                currentUser!.FirstName = request.FirstName;
                currentUser!.LastName = request.LastName;
                currentUser!.Email = request.EmailAddress;
                currentUser!.PhoneNumber = request.Phone;
                currentUser!.City = request.City;
                currentUser!.BirthDate = request.BirthDate;
                currentUser!.Gender = request.GenderType;
                //upload a photo to wwwroot file

                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                var folderName = wwwrootFolder!.First(x => x.Name == "img").Name;

                var servicePathWithFolderName = Path.Combine($"{Request.Scheme}://{Request.Host}{Request.PathBase}/{folderName}/");

                if (request.Photo != null || request.Photo?.Length > 0)
                {

                    //delete previous file from wwwroot

                    if(!string.IsNullOrEmpty(currentUser.Picture))
                    {
                        string existingFile = Path.Combine(_webHostEnvironment.WebRootPath,"img",currentUser.Picture);
                        System.IO.File.Delete(existingFile);
                    }
                    string randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Photo.FileName)}";

                    var newPicturepath = Path.Combine(wwwrootFolder!.First(x => x.Name == "img").PhysicalPath!, randomFileName);
                    using var stream = new FileStream(newPicturepath, FileMode.Create);

                    await request.Photo.CopyToAsync(stream);

                    currentUser!.Picture = randomFileName;

                    request.PhotoSrc = Path.Combine(servicePathWithFolderName, randomFileName);

                }
                else
                    request.PhotoSrc = Path.Combine(servicePathWithFolderName, currentUser!.Picture!);


                IdentityResult updateResult =  await _userManager.UpdateAsync(currentUser);

                if (!updateResult.Succeeded)
                    ModelState.AddModelStateError(updateResult.Errors.ToArray());


                //SecurityStamp değerini güncelledik.Çünkü kullanıcı değerlerinde değişikliğe gittik.
                await _userManager.UpdateSecurityStampAsync(currentUser);
                //Çıkış yaptık.
                await _signInManager.SignOutAsync();
                //Çıkış yaptıktan sonra tekrar yeni şifre ile giriş yaptık.

                if (currentUser.BirthDate.HasValue)
                    await _signInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim(ClaimTypes.DateOfBirth,currentUser.BirthDate.Value.ToString()) });
                else
                    await _signInManager.SignInAsync(currentUser,true);

                return View(nameof(HomeController.Profile),request);

            }

            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(PasswordChangeViewModel request)
        {
            if (ModelState.IsValid) 
            {
                var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

                bool checkOldPassword = await _userManager.CheckPasswordAsync(currentUser,request.OldPassword);

                if (!checkOldPassword)
                    ModelState.AddModelStateError(new IdentityError[] { new IdentityError { Code = string.Empty, Description = "Current password is wrong!" } });

                IdentityResult result = await _userManager.ChangePasswordAsync(currentUser, request.OldPassword, request.NewPassword);

                if (!result.Succeeded)
                {
                    ModelState.AddModelStateError(result.Errors.ToArray());
                    return View();
                }
                else
                {
                    TempData["SuccessMessage"] = "Password has been successfully changed!";

                    //SecurityStamp değerini güncelledik.Çünkü kullanıcı değerlerinde değişikliğe gittik.
                    await _userManager.UpdateSecurityStampAsync(currentUser);
                    //Çıkış yaptık.
                    await _signInManager.SignOutAsync();
                    //Çıkış yaptıktan sonra tekrar yeni şifre ile giriş yaptık.
                    await _signInManager.PasswordSignInAsync(currentUser, request.NewPassword, true, false);
                }
            }
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "Over18Only")]
        public IActionResult OverEighteenOnly()
        {
            return View();
        }

    }
}
