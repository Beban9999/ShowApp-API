using System;
namespace AppApi.Models.Post
{
	public class PostMediaRequest
	{
        public int Id { get; set; }
        public IFormCollection? Files { get; set; }
    }
}

