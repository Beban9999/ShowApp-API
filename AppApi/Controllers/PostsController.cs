using AppApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        private readonly string _connectionString;

        public PostsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
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
    }
}
