namespace AppApi.Models
{
    using System.Collections.Generic;

    public class UpdateRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ArtistName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public List<int> Genre { get; set; }
        public int UserId { get; set; }
        public bool IsArtist { get; set; }
    }
}
