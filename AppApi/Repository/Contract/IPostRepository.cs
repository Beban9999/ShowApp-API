using AppApi.Models;
using AppApi.Models.Post;

namespace AppApi.Repository.Contract
{
    public interface IPostRepository
    {
        public RequestResponse InsertPost(PostsRequest postsRequest);
        public List<Post> Get_Post(int? id);
    }
}
