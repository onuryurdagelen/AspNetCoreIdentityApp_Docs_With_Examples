
using AspNetCoreIdentityApp.Core.Models;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class ProfileEditViewModel
    {
        public string UserId { get; set; } = null!;
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
        public string? Phone { get; set; }
        public string EmailAddress { get; set; } = null!;

        public string? PhotoSrc { get; set; }

        public IFormFile? Photo { get; set; }
        public string? City { get; set; }
        public DateTime? BirthDate { get; set; }
        public GenderType? GenderType { get; set; }
    }
}
