using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace BlogWebApp.Models.IdentityModel
{
    public class User : IdentityUser
    {
        public string? DisplayName { get; set; }
        [MaxLength(500, ErrorMessage = "Your Bio must be at most 500 characters")]
        public string? Bio { get; set; }
        public string? Position { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? ProfileUrl { get; set; }
        //make a alter table command

    }
}
