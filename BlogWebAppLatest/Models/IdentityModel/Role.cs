using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Models.IdentityModel
{
    public class Role : IdentityRole
    {
        public string AliasName { get; set; }
    }
}
