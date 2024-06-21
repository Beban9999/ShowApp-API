using System;
namespace AppApi.Models.Artist
{
	public class ArtistMedia
	{
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public int PostId { get; set; }
        public long UserId { get; set; }
    }
}

