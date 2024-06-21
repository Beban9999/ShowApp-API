using System;
namespace AppApi.Models.Artist
{
	public class ArtistPost
	{
        public int Id { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
        public int UserId { get; set; }
        public List<ArtistMedia> Media { get; set; }
    }
}

