using AspNetCoreIdentityApp.Web.Areas.Client.Models;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Web.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Areas.Client.Controllers
{
    [Area("Client")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, EmailService emailService, IWebHostEnvironment env, IConfiguration configuration, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _env = env;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel request,string? returnUrl = null)
        {

            returnUrl ??= Url.Action("Index", "Home", new {area =""});
            //returnUrl null(önceki sayfa yok ise) ise ana sayfaya yönlendirme yaparız
            //değerler değil ise
            if (ModelState.IsValid)
            {
                //beni hatırla etkinleştirilmişse kullanıcı tekrar giriş yaptığında giriş sayfasına yönlendirmek için cookie oluşturmalıyız.
                //hash => Password12* => asdmasd12*=((^'
                AppUser hasUser = await _userManager.FindByEmailAsync(request.EmailAddress);

                //kullanıcı yok ise
                if (hasUser == null)
                {
                    ModelState.AddModelStateError(new IdentityError[] {new IdentityError { Code = string.Empty,Description = "Email or password is wrong!" } });
                    return View();
                }

                //kullanıcı var ise
                //isPersistent => kullanıcı bilgilerinin cookie de tutulup tutulmadığını belirtler.true => otomatik olarak cookie oluşturur.
                //lockoutOnFailure => kilitleme mekanizmasını çalıştırıyoruz.
                var signInResult = await _signInManager.PasswordSignInAsync(hasUser, request.Password, request.RememberMe, true);

                //kullanıcı kilitlenmiş ise
                if (signInResult.IsLockedOut)
                {
                    ModelState.AddModelStateError(new IdentityError[] { new IdentityError { Code = string.Empty, Description = "You cannot login for 3 minutes!" } });
                    return View();
                }

                if (!signInResult.Succeeded)
                {
                    var accessFailedCount = await _userManager.GetAccessFailedCountAsync(hasUser);
                    ModelState.AddModelStateError(new IdentityError[] 
                    { new IdentityError { Code = string.Empty, Description = "Email or password is wrong!" },
                    new IdentityError{ Code = string.Empty,Description = $"Failed access count: {accessFailedCount}"}
                    });
                    return View();
                }

                if (hasUser.BirthDate.HasValue)
                    await _signInManager.SignInWithClaimsAsync(hasUser, request.RememberMe, new[] { new Claim(ClaimTypes.DateOfBirth,hasUser.BirthDate.Value.ToString())});
                

                if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                else
                    return RedirectToAction(nameof(HomeController.Index), "Home",new {area = ""});

            }
            ViewData["ReturnUrl"] = returnUrl;
            //değerler valid değil ise
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel request)
        {

            //değerler valid değil ise
            if (ModelState.IsValid)
            {
                //hash => Password12* => asdmasd12*=((^'
                IdentityResult registerResult = await _userManager.CreateAsync(
                    new() { 
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        UserName = request.FirstName.ToLower()+request.LastName.ToLower(),
                        Email = request.EmailAddress, 
                        PhoneNumber = request.Phone }
                    , 
                    request.PasswordConfirm);

                //sonuç başarılı değilse
                if (!registerResult.Succeeded)
                {
                    ModelState.AddModelStateError(registerResult.Errors.ToArray());
                    return View(request);

                }

                //add a role to the created user(default role :User)


                var registeredUser = await _userManager.FindByEmailAsync(request.EmailAddress);

                IdentityResult roleResult =  await _userManager.AddToRoleAsync(registeredUser!, "User");

                //sonuç başarılı değilse
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelStateError(roleResult.Errors.ToArray());
                    return View(request);

                }

                //sonuç başarılı değilse
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelStateError(roleResult.Errors.ToArray());
                    return View(request);

                }



                IdentityResult claimResult =  await _userManager.AddClaimAsync(registeredUser!, new Claim("PageExpireDate", DateTime.UtcNow.AddDays(double.Parse(_configuration.GetSection("ExchangeExpireClaim").Value)).ToString()));

                //sonuç başarılı değilse
                if (!claimResult.Succeeded)
                {
                    ModelState.AddModelStateError(claimResult.Errors.ToArray());
                    return View(request);

                }
                TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarıyla gerçekleştirilmiştir.";
                    return RedirectToAction(nameof(HomeController.Register));

            }
            //değerler valid değil ise
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token,string email)
        {
            return View(new ResetPasswordViewModel { EmailAddress = email,Token = token});
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
         

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.EmailAddress))
                {
                    ModelState.AddModelStateError(new IdentityError[] { new IdentityError { Code = string.Empty, Description = "Invalid token or email address" } });
                    return View();
                }
                AppUser hasUser = await _userManager.FindByEmailAsync(request.EmailAddress);

                //kullanıcı yok ise
                if (hasUser == null)
                {
                    ModelState.AddModelStateError(new IdentityError[] { new IdentityError { Code = string.Empty, Description = "Invalid email address!" } });
                    return View();
                }
                IdentityResult result = await _userManager.ResetPasswordAsync(hasUser, request.Token, request.Password);

                if (!result.Succeeded)
                {
                    ModelState.AddModelStateError(result.Errors.ToArray());
                    return View();
                }

                return RedirectToAction(nameof(HomeController.PasswordConfirmation), new { area = "Client" });
            }
            return View();
            
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel request)
        {

            if (ModelState.IsValid) 
            {
                //get the user by email address

                var user = await _userManager.FindByEmailAsync(request.EmailAddress);
                if (user == null)
                {
                    ModelState.AddModelStateError(new IdentityError[] { new IdentityError { Code = string.Empty, Description = "User not found!" } });
                    return View();
                }

                //first of all get html templates from the wwwroot file
                var templatePath = Path.Combine(_env.WebRootPath, "templates/html-templates/resetPassword.html");
                var template = await System.IO.File.ReadAllTextAsync(templatePath);

                //create reset password token from userManager
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetLink = Url.Action("ResetPassword", "Home", new { area = "Client", token = token, email = request.EmailAddress }, Request.Scheme);

                var message = template.Replace("{{resetLink}}", resetLink);

                bool result = await _emailService.SendEmailAsync(request.EmailAddress, "Reset Password", message);

                //if result true 
                if(result)
                    TempData["SuccessMessage"] = "Successfully has been sent to the email to reset password";
                else
                    TempData["ErrorMessage"] = "Something went wrong whilst sending email to reset password";


            }
            return View();
        }

        [HttpGet]
        public IActionResult PasswordConfirmation()
        {
            return View();
        }
        
    }
}
