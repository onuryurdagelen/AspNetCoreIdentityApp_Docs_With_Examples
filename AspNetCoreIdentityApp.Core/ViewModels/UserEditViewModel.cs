using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class UserEditViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }

        public string? Phone { get; set; }

        [Display(Name ="Selected Roles")]
        public IEnumerable<string>? SelectedRoles { get; set; }
        public IEnumerable<SelectListItem>? RolesList { get; set; }
    }
}
