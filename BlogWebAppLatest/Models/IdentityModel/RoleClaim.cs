using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Models.IdentityModel
{
    public class RoleClaim : IdentityRoleClaim<string>
    {
        public int? Id { get; set; }
    }
}
