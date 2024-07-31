using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class AddRoleViewModel
    {
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }
}
