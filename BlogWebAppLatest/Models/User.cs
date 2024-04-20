using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlogWebApp.Models
{
    public class User: IdentityUser
    {
        public string? DisplayName { get; set; }
    }
}
