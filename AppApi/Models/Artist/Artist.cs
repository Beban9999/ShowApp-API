using System;
using AppApi.Models.Post;

namespace AppApi.Models.Artist
{
	public class Artist
	{
        public Artist()
        {
            Genres = new List<string>();
            Posts = new List<ArtistPost>();
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Avatar { get; set; }
        public List<string> Genres { get; set; }
        public List<ArtistPost> Posts { get; set; }
    }
}

