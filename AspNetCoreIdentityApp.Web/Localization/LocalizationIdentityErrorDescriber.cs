using Microsoft.AspNetCore.Identity;
using System.Net.Mail;

namespace AspNetCoreIdentityApp.Web.Localization
{
    public class LocalizationIdentityErrorDescriber:IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new() { Code = "DuplicateUserName", Description = $"{userName} daha önce başka bir kullanıcı tarafından alınmıştır." };
            //return base.DuplicateUserName(userName);
        }
        public override IdentityError DuplicateEmail(string emailAddress)
        {
            return new() { Code = "DuplicateEmail", Description = $"{emailAddress} daha önce başka bir kullanıcı tarafından alınmıştır." };
            //return base.DuplicateUserName(userName);
        }
        public override IdentityError PasswordTooShort(int length)
        {
            return new() { Code = "PasswordTooShort", Description = $"Şifre en az {length} karakterli olmalıdır." };
        }
    }
}
