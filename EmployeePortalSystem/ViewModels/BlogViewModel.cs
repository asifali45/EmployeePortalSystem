using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortalSystem.ViewModels
{
    public class BlogViewModel
    {
        public int? BlogId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string? Image { get; set; }

        public IFormFile? ImageFile { get; set; } // For uploading image files

        public string Tags { get; set; }

        public int? AuthorId { get; set; } // EmployeeId of the author
        public string? AuthorName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int LikeCount { get; set; }
       

    
       // For comments
        public List<BlogCommentViewModel> Comments { get; set; } = new List<BlogCommentViewModel>();

        public string? NewCommentText { get; set; } = "";
    }

    public class BlogCommentViewModel
    {
        public int CommentId { get; set; }
        public int BlogId { get; set; }
        public int EmployeeId { get; set; }
        public string? CommentText { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EmployeeName { get; set; } // To display commenter's name
    }
}
