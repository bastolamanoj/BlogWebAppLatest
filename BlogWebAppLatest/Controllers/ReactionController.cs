using BlogWebApp.Models;
using BlogWebApp.Models.IdentityModel;
using BlogWebApp.ViewModel;
using BlogWebAppLatest.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Controllers
{
    public class ReactionController : Controller
    {
      private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        public ReactionController(ApplicationDbContext dbcontext, UserManager<User> userManager)
        {
            _dbContext = dbcontext;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReactionStatus([FromBody] ReactionVM reactions)
        {
            if (reactions == null)
            {
                return BadRequest("Reaction data is null");
            }
            Reaction reaction = new Reaction()
            {
                EntityType = reactions.EntityType,
                Type= reactions.Type,
                EntityId = Guid.Parse(reactions.EntityId)

            };
            var user = _userManager.GetUserAsync(User).Result;
            //var user = await _userManager.GetUserAsync(User);
            var userid = user.Id;

            reaction.UserId = (user == null ? Guid.Empty : (Guid.TryParse(userid, out var userId) ? userId : Guid.Empty));
           
            try
            {
                // Check if the user has already reacted to the entity
                var existingReaction = await _dbContext.Reactions
                    .FirstOrDefaultAsync(r => r.UserId == reaction.UserId && r.EntityId == reaction.EntityId);

                if (existingReaction != null)
                {
                    reaction.CreationDate = DateTime.Now;
                    // Update the existing reaction
                    existingReaction.Type = reaction.Type;
                    _dbContext.Reactions.Update(existingReaction);
                }
                else
                {
                    // Insert a new reaction
                    reaction.CreationDate = DateTime.Now;
                    _dbContext.Reactions.Add(reaction);
                }

                await _dbContext.SaveChangesAsync();
                return Ok(); // Return success response
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
