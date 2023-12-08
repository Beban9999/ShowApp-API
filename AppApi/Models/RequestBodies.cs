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

    public class PostsRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }  
        public decimal? Price { get; set; }
    }
}
