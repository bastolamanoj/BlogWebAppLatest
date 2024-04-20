using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Models
{
    public class RoleClaim: IdentityRoleClaim<string>
    {
        public int? Id { get; set; }
    }
}
