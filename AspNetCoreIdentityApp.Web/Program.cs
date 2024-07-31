using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Service.Services;
using AspNetCoreIdentityApp.Web.AuthorizationRequirements;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Middlewares;
using AspNetCoreIdentityApp.Web.Permissions;
using AspNetCoreIdentityApp.Web.Seeds;
using AspNetCoreIdentityApp.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("tr"),
        new CultureInfo("en")
    };

    options.DefaultRequestCulture = new RequestCulture("tr");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

builder.Services.AddTransient<EmailService>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"),options =>
    {
        options.MigrationsAssembly("AspNetCoreIdentityApp.Repository");
    });
});
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(30); //30 dakikada bir SecurityStamp deðerini Cookie'den alýp veritabaný ile karþýlaþtýrýr.Ayný deðil ise tekrardan login olman gerekir.
});
//Implement IdentityCore => StartupExtensions -> AddIdentityWithExtension() method
builder.Services.AddIdentityWithExtension();

//Session Configuration


builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
builder.Services.AddScoped<IMemberService,MemberService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Over18Only", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(claimType: ClaimTypes.DateOfBirth);
        policy.AddRequirements(new MinimumAgeRequirement(18));
    });


    options.AddPolicy("OrderPermissionReadAndDelete", policy =>
    {

        policy.RequireClaim("permission", Permissions.Order.Read);
        policy.RequireClaim("permission", Permissions.Order.Delete);
        policy.RequireClaim("permission", Permissions.Stock.Delete);

    });



    options.AddPolicy("Permissions.Order.Read", policy =>
    {

        policy.RequireClaim("permission", Permissions.Order.Read);


    });

    options.AddPolicy("Permissions.Order.Delete", policy =>
    {

        policy.RequireClaim("permission", Permissions.Order.Delete);


    });


    options.AddPolicy("Permissions.Stock.Delete", policy =>
    {

        policy.RequireClaim("permission", Permissions.Stock.Delete);


    });

});

//Coookie Configuration
builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder();

    cookieBuilder.Name = "IdentityAppCookie";
    opt.LoginPath = new PathString("/Client/Home/Login");
    opt.LogoutPath = new PathString("/Client/Member/Logout");
    opt.AccessDeniedPath = new PathString("/Client/Member/AccessDenied");
    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60);
    opt.SlidingExpiration = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

    await PermissionSeed.Seed(roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
