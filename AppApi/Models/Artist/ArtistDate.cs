using System;
namespace AppApi.Models.Artist
{
	public class ArtistDate
	{
		public int UserId { get; set; }
		public List<DateTime> Dates { get; set; }
	}
}

