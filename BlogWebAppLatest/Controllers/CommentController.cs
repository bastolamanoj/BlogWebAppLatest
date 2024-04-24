using BlogWebApp.Models;
using BlogWebApp.Models.IdentityModel;
using BlogWebAppLatest.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<User> _userManager;
        public CommentController(ApplicationDbContext dbcontext, UserManager<User> userManager) { 
            _dbcontext = dbcontext;  
            _userManager= userManager;
        }  
 
        [HttpPost]
        public  async Task<ActionResult> AddNestedReply([FromBody] CommentReply commentReply)
        {
            var user = await _userManager.GetUserAsync(User);
            var userid = user.Id;
            // Create a new reply object
            var reply = new CommentReply
            {
                Content = commentReply.Content,
                CommentId = commentReply.CommentId,
                //ParentReplyId = 0,
                AuthorId = Guid.Parse(userid), 
                Timestamp = DateTime.Now
            };

            // Save the reply to the database
            _dbcontext.CommentReplies.Add(reply);
            _dbcontext.SaveChanges();
            return Ok(new { status = "success", message = "Reply added successfully" });
            // Redirect the user back to the blog details page or return JSON response indicating success
        }

        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] Comment comment)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            var user = _userManager.GetUserAsync(User).Result;
            //var user = await _userManager.GetUserAsync(User);
            var userid = user.Id;
        
            comment.CommentedBy =Guid.Parse(userid) ; // Replace with actual user ID

            comment.CreationDate = DateTime.Now;
            //comment.AUt

            _dbcontext.Comments.Add(comment);
            await _dbcontext.SaveChangesAsync();

            return Ok(new {status="200",message="success"}); // Return success

        }

        [HttpPost]
        public async Task<IActionResult> UpdateComment([FromBody] Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingComment = await _dbcontext.Comments.FindAsync(comment.Id);
            if (existingComment == null)
            {
                return NotFound("Comment not found.");
            }

            existingComment.Content = comment.Content;
            // Update other properties as needed

            _dbcontext.Comments.Update(existingComment);
            await _dbcontext.SaveChangesAsync();

            return Ok(new { status = "200", message = "success" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var comment = await _dbcontext.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound("Comment not found.");
            }

            _dbcontext.Comments.Remove(comment);
            await _dbcontext.SaveChangesAsync();

            return Ok(new { status = "200", message = "success" });
        }
    }
}
