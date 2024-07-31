using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Service.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;

        public MemberService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserViewModel> GetUserViewModelByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserViewModel>> GetUserViewModelList()
        {
            var users = await _userManager.Users.ToListAsync();

            var userWithRolesList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userWithRolesList.Add(new UserViewModel
                {
                    EmailAddress = user.Email,
                    FullName = user.FullName,
                    Phone = user.PhoneNumber,
                    UserId = user.Id,
                    Roles = String.Join(",", roles)
                });
            }
            return userWithRolesList;
        }

        public Task Login()
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }
    }
}
