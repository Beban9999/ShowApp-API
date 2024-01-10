namespace AppApi.Models.Post
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public List<PostImage>? Images { get; set; }
    }
}
