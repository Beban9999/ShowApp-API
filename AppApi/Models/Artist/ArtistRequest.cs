using System;
namespace AppApi.Models.Artist
{
	public class ArtistRequest
	{
        public ArtistRequest(int userId, string name, int typeId, decimal price, int[] genre, string description, string location)
        {
            UserId = userId;
            Name = name;
            TypeId = typeId;
            Price = price;
            Genre = genre;
            Description = description;
            Location = location;
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public decimal Price { get; set; }
        public int[] Genre { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }


    }
}

