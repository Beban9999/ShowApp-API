using AppApi.Models;
using AppApi.Models.Post;
using AppApi.Repository.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        private readonly IPostRepository _postRepository;

        public PostsController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        
        [HttpGet("get")]
        public ActionResult<Response> Get_Posts(int? id)
        {
            Response response = new Response();
            try
            {
                List<Post> posts = _postRepository.Get_Post(id);
                response.Data = JsonConvert.SerializeObject(posts);
                response.Status = RequestStatus.Success;
                return Ok(response);
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Status = RequestStatus.Error;
                return BadRequest(response);
            }
        }

        [HttpPost("insert")]
        public ActionResult<Response> Insert_Post([FromBody] Post postsRequest)
        {
            Response response = new Response();
            try
            {
                RequestResponse resp = _postRepository.InsertPost(postsRequest);
                if (resp.IsSuccessfull)
                {
                    response.Data = JsonConvert.SerializeObject(resp.Result);
                    response.Message = "Post successfully inserted!";
                    response.Status = RequestStatus.Success;
                    return Ok(response);
                }
                else
                {
                    response.Data = JsonConvert.SerializeObject(false);
                    response.Message = resp.ErrorMessage;
                    response.Status = RequestStatus.Error;
                    return BadRequest(response);
                }
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Status = RequestStatus.Error;
                return BadRequest(response);
            }
        }

    }
}
