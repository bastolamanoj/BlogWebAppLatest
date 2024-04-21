using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogWebAppLatest.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string,
        UserClaim, UserRole, UserLogin,
    RoleClaim, UserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {


        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set;    }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("dbo");

            //builder.Entity<ApplicationUser>().ToTable("ApplicationUsers");
            //builder.Entity<ApplicationUser>(e=> { e.ToTable(name: "ApplicationUsers", schema: "identity"); });
            
            //Identity user section

            builder.Entity<User>(e => { e.ToTable(name: "Users"); });
            builder.Entity<Role>(e => { e.ToTable(name: "Roles"); });
            builder.Entity<UserRole>(e => { e.ToTable(name: "UserRoles"); });
            builder.Entity<RoleClaim>(e => { e.ToTable(name: "RoleClaims"); });
            builder.Entity<UserClaim>(e => { e.ToTable(name: "UserClaims"); });
            builder.Entity<UserLogin>(e => { e.ToTable(name: "UserLogins"); });
            builder.Entity<UserToken>(e => { e.ToTable(name: "UserTokens"); });

            // Other Custom model entity section


            //builder.Entity<ApplicationRole>().ToTable("ApplicationRoles");
            //builder.Entity<ApplicationRoleClaims>().ToTable("ApplicationRoleClaims");
            //builder.Entity<ApplicationUserClaim>().ToTable("ApplicationUserClaims");
            //builder.Entity<IdentityUserLogin<string>>().ToTable("ApplicationUserLogins");
            //builder.Entity<IdentityUserToken<string>>().ToTable("ApplicationUserTokens");

        }


    }
}
