using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Seeds
{
    public class PermissionSeed
    {
        public static async Task Seed(RoleManager<AppRole> roleManager)
        {
            var hasUserRole = await roleManager.RoleExistsAsync("User");
            var hasEditorRole = await roleManager.RoleExistsAsync("Editor");
            var hasAdminRole = await roleManager.RoleExistsAsync("Administrator");

            if (!hasUserRole)
            {

                await roleManager.CreateAsync(new AppRole() { Name = "User" });


                var userRole = (await roleManager.FindByNameAsync("User"))!;

                await AddReadPermission(userRole, roleManager);
            }

            if (!hasEditorRole)
            {

                await roleManager.CreateAsync(new AppRole() { Name = "Editor" });


                var editorRole = (await roleManager.FindByNameAsync("Editor"))!;

                await AddReadPermission(editorRole, roleManager);
                await AddUpdateAndCreatePermission(editorRole, roleManager);
            }


            if (!hasAdminRole)
            {

                await roleManager.CreateAsync(new AppRole() { Name = "Administrator" });


                var adminRole = (await roleManager.FindByNameAsync("Administrator"))!;

                await AddReadPermission(adminRole, roleManager);
                await AddUpdateAndCreatePermission(adminRole, roleManager);
                await AddDeletePermission(adminRole, roleManager);
            }
        }
        public static async Task AddReadPermission(AppRole role, RoleManager<AppRole> roleManager)
        {

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Stock.Read));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Order.Read));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Catalog.Read));

        }

        public static async Task AddUpdateAndCreatePermission(AppRole role, RoleManager<AppRole> roleManager)
        {

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Stock.Create));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Order.Create));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Catalog.Create));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Stock.Update));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Order.Update));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Catalog.Update));

        }
        public static async Task AddDeletePermission(AppRole role, RoleManager<AppRole> roleManager)
        {

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Stock.Delete));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Order.Delete));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Catalog.Delete));

        }
    }
}
