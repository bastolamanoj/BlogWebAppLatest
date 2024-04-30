using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing.Constraints;
using System.IO;
using BlogWebApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using BlogWebApp.Models;
using System.Text;
using BlogWebApp.Models.IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Controllers
{
    //[Route("api/[controller]/[Action]")]
    //[ApiController]
    [Authorize]
    public class FileController : Controller
    {
        private static IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<User> _userManager;

        //public class BlogFileInfo
        //{
        //    public string FileName { get; set; }
        //    public string BlogTitle { get; set; }
        //    public DateTime Date { get; set; }
        //}
        public FileController(IWebHostEnvironment webHostEnvironment, UserManager<User> userManager)
        {
            _webHostEnvironment = webHostEnvironment;
            _userManager= userManager;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] FileModel filemodel)
        {
            try
            {
                if(filemodel.File.Length > 0)
                {
                    var path = _webHostEnvironment.WebRootPath + "\\uploads\\"; 
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);    
                    }
                    var filename = Guid.NewGuid().ToString() + filemodel.File.FileName;
                    using (FileStream filestream = System.IO.File.Create(path + filename))
                    {
                        filemodel.File.CopyTo(filestream);
                        filestream.Flush();
                        return Ok(filename);
                    }
                }
                else
                {
                    return BadRequest("Failed");
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status417ExpectationFailed, ex.Message);

            }

        }

        [HttpGet("{filename}")]
        public async Task<IActionResult> GetFile([FromRoute] string filename)
        {
            string path = _webHostEnvironment.WebRootPath + "\\uploads\\";
            string filepath = path + filename;
            string ext = Path.GetExtension(filepath);
            if (System.IO.File.Exists(filepath))
            {
                //byte[] b= System.IO.File.ReadAllBytes(filepath);
                return Ok(filepath);    
            }
            return null;
        }

        [HttpDelete("{filename}")]
        public async Task<IActionResult> DeleteFile([FromRoute] string filename) 
        {
            try
            {
                string path = _webHostEnvironment.WebRootPath + "\\uploads\\";
                string filepath = path + filename;
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                    return Ok(filepath);

                }
                return null;
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

        [HttpPost]
        public ActionResult SaveBlogFile(BlogVM blog)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;

                if (user == null)
                {
                    return NotFound();
                }
                // Generate file name based on current date and blog title
                //string fileName = $"{DateTime.Now:yyyy-MM-dd}-{blog.Title}.txt";
                string fileName = $"({DateTime.Now:yyyy-MM-dd})-{user.Id}-{blog.Title}.txt";
                // Get the physical path to the application's directory
                var path = _webHostEnvironment.WebRootPath + "\\BlogLogs\\";
                DateTime currentDate = DateTime.Now;

                // Ensure the directory exists, create it if not
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Combine the directory path and file name to get the full file path
                string filePath = Path.Combine(path, fileName);
              
                // Write blog content to the file
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    writer.WriteLine($"Title: {blog.Title}");
                    writer.WriteLine($"Author: {user.Id}");
                    writer.WriteLine($"Author: {user.DisplayName}");
                    writer.WriteLine($"Date: {currentDate}");
                    writer.WriteLine($"Body:\n{blog.Body}");
                    writer.WriteLine($"Body:\n{blog.Action}");
                }

                // Return success message or redirect to another action
                return Content($"File '{fileName}' created successfully!");
            }
            catch (Exception ex)
            {
                // Handle any errors
                return Content($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public ActionResult GetUserLog()
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;

                if (user == null)
                {
                    return NotFound();
                }
                // Get the physical path to the application's directory
                var path = _webHostEnvironment.WebRootPath + "\\BlogLogs\\";

                // Ensure the directory exists
                if (Directory.Exists(path))
                {
                    // Get all files in the directory
                    string[] files = Directory.GetFiles(path);
                    var fileNamePattern = $"*-{user.Id}-*.txt";
                    // Filter files based on user ID and extract file names
                    var userLogFiles = Directory.EnumerateFiles(path, fileNamePattern)
                                         .Select(file => new BlogFileInfo
                                         {
                                             FileName = Path.GetFileName(file),
                                             BlogTitle = ExtractBlogTitleFromFileName(file),
                                             Url= Path.GetFileName(file),
                                             Date = ExtractCreationDateFromFileName(Path.GetFileName(file)) // Implement your date extraction logic here
                                         });

                    // Return the list of log file names for the user
                    return View(userLogFiles);
                }
                else
                {
                    return Content("Log directory does not exist.");
                }
            }
            catch (Exception ex)
            {
                // Handle any errors
                return Content($"An error occurred: {ex.Message}");
            }
        }


        private string ExtractCreationDateFromFileName(string fileName)
        {
            // Extract creation date from the file name
            // Assuming file name format is "yyyy-MM-dd-{BlogTitle}.txt"
            string[] parts = fileName.Split('-');
            if (parts.Length >= 2)
            {
                //if (DateTime.TryParse(parts[0] + "-" + parts[1], out DateTime creationDate))
                //{
                return parts[0] + "-" + parts[1] + "-" + parts[2];
                //}
            }
            return parts[0]+"-" + parts[1] + "-" + parts[2];
        }

        private string ExtractBlogTitleFromFileName(string fileName)
        {
            // Extract blog title from the file name
            // Assuming file name format is "yyyy-MM-dd-{BlogTitle}.txt"
            string[] parts = fileName.Split('-');
            if (parts.Length >= 3)
            {
                return string.Join("-", parts.Skip(8)).Replace(".txt", "");
            }
            return null; // Or handle the case where blog title cannot be extracted
        }
    }
}
