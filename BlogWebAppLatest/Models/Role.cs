using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Models
{
    public class Role: IdentityRole
    {
        public string AliasName { get;set; }
    }
}
