using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Areas.Client.Models
{
    public class LoginViewModel
    {

        public LoginViewModel()
        {
            
        }

        public LoginViewModel(string emailAddress, string password)
        {
            EmailAddress = emailAddress;
            Password = password;
        }

        [Required(ErrorMessage = "Email Address cannot be empty.")]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress,ErrorMessage = "Email Address format is wrong.")]
        public string EmailAddress { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password cannot be empty.")]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }

    }
}
