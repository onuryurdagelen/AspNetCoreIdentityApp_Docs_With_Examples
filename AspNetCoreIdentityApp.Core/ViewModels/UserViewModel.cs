
namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class UserViewModel
    {
        public string UserId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
        public string EmailAddress { get; set; } = null!;
        public string? Roles { get; set; }

    }
}
