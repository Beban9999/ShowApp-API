using System;
namespace AppApi.Models.Artist
{
	public class ArtistMediaRequest
	{
        public int Id { get; set; }
        public bool IsProfile { get; set; }
        public IFormCollection? Files { get; set; }
        public int PostId { get; set; }
    }
}

