namespace EmployeePortalSystem.ViewModels
{
    public class BlogViewModel
    {
        public int BlogId { get; set; }
        public string Title { get; set; }

        public string Content { get; set; }

        public string Image { get; set; }

        public string Tags { get; set; }

        public string AuthorName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int LikeCount { get; set; }
       

    }
}
