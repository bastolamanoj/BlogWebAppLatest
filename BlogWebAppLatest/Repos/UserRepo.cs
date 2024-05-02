using BlogWebApp.Models.IdentityModel;
using BlogWebAppLatest.Data;
using Microsoft.EntityFrameworkCore;

namespace SignalRYoutube.Repos
{
    public class UserRepo
    {
        private readonly ApplicationDbContext dbContext;   

        public UserRepo(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<User> GetUserDetails(string username, string password)
        {
            return await dbContext.Users.FirstOrDefaultAsync(user => user.DisplayName == username && user.PasswordHash == password);
        }
    }
}
