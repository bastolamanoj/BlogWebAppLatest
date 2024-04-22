using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModel
{
    public class BlogVM
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
        public int? BlogCategoryId { get; set; }
        public List<BlogImageVM> BlogImages { get; set; }
    }

    public class BlogImageVM {
        public string? ImageName { get; set; }
        public string Url { get; set; }
    }


}
