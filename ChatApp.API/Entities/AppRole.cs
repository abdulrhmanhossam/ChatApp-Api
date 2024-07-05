using Microsoft.AspNetCore.Identity;

namespace ChatApp.API.Entities;
public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}
