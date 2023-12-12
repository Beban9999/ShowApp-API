namespace AppApi.Models.Post
{
    public class Post
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public List<PostImage>? Images { get; set; }
    }
}
