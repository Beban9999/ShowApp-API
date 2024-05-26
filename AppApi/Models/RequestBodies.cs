using AppApi.Models.Post;

namespace AppApi.Models
{
    public class LoginRequest
    {
        public string LoginName { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest : LoginRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
    public class UserStatusRequest
    {
        public bool IsActive { get; set; }
        public int UserID { get; set; }
    }

    public class UserData
    {
        public int UserId { get; set; }
        public string LoginName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }   
        public bool IsActive { get; set; }
        public bool isDeleted {  get; set; }
    }

    public class PostsRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }  
        public decimal? Price { get; set; }
    }
    public class Artist
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public List<PostMedia> PostMedias { get; set; }
    }

    //public class PostMedia
    //{
    //    public string FileName { get; set; }
    //    public string FileType { get; set; }
    //    public long PostId { get; set; }
    //}
}
