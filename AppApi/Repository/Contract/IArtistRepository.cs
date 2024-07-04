using System;
using AppApi.Models;
using AppApi.Models.Artist;

namespace AppApi.Repository.Contract
{
	public interface IArtistRepository
	{
        public List<Artist> GetArtists(int? userId);
        public RequestResponse BecomeArtist(ArtistRequest request);
        public List<ArtistType> GetArtistTypes();
        public List<ArtistGenre> GetArtistGenres();
        public RequestResponse InsertPost(ArtistPostRequest post);
        public RequestResponse RemovePost(int postId);
        public RequestResponse RemoveArtist(int userId);
        public RequestResponse UpdateArtist(UpdateRequest request);
        public RequestResponse InsertDate(ArtistDate request);
    }
}

