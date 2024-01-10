using AppApi.Models;
using AppApi.Models.Post;
using AppApi.Repository.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace AppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        private readonly string _connectionString;
        private readonly IPostRepository _postRepository;

        public PostsController(IConfiguration configuration, IPostRepository postRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _postRepository = postRepository;
        }

        #region old_calls
        [HttpPost]
        public async Task<int> InsertPost([FromBody] PostsRequest postsRequest)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("usp_InsertPost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Title", postsRequest.Title);
                    command.Parameters.AddWithValue("@Description", postsRequest.Description);
                    command.Parameters.AddWithValue("@Price", postsRequest.Price);

                    var postID = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return postID;
                }
            }
        }
        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts([FromQuery] int? id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("usp_GetPosts", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    // Add the @PostID parameter to filter by ID
                    command.Parameters.AddWithValue("@PostID", id ?? (object)DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            // Read the JSON data and return it as a JSON response
                            while (await reader.ReadAsync())
                            {
                                var jsonString = reader.GetString(0); // Assuming the JSON data is in the first column
                                return Content(jsonString, "application/json");
                            }
                        }
                    }

                    return NotFound(); // Handle the case where no data is found
                }
            }
        }
        #endregion

        #region new calls
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
                    response.Data = JsonConvert.SerializeObject(true);
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

        #endregion

    }
}
