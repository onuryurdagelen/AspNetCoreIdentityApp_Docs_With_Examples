using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Repository.Models
{
    public class AppDbContext:IdentityDbContext<AppUser,AppRole,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
          
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>().Property(x => x.FullName).HasComputedColumnSql("[FirstName] +' '+ [LastName]");
            base.OnModelCreating(builder);
        }
        
    }
}
