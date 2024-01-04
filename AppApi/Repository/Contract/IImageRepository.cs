using AppApi.Models;

namespace AppApi.Repository.Contract
{
    public interface IImageRepository
    {
        public RequestResponse UploadImage(List<IFormFile> imageDate, int postId);
        public RequestResponse UploadMedia(List<IFormFile> imageDate, int postId);
        public byte[] Get_Image(int imageId);
    }
}
