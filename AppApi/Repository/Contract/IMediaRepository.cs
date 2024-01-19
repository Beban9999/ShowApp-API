using System;
using AppApi.Models;

namespace AppApi.Repository.Contract
{
	public interface IMediaRepository
	{
        public RequestResponse UploadMedia(IFormFileCollection? imageDate, int postId);

    }
}

