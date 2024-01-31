namespace AppApi.Models.Post
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string User { get; set; }
        public List<PostMedia>? Medias { get; set; }
        public IFormCollection? Files { get; set; }
    }
}
