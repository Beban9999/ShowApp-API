using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace AppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly string _connectionString;

        public ImagesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        [HttpPost("{post_id}")]
        public async Task<IActionResult> UploadImages(List<IFormFile> images, int post_id)
        {
            if (images == null || images.Count == 0)
            {
                return BadRequest("No image files uploaded.");
            }
            var test = post_id;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var image in images)
                        {
                            using (SqlCommand command = new SqlCommand("INSERT INTO Images (ImageData, FileName, PostID) VALUES (@ImageData, @FileName, @PostId); SELECT SCOPE_IDENTITY();", connection, transaction))
                            {
                                command.Parameters.Add("@ImageData", SqlDbType.VarBinary, -1).Value = ReadFully(image.OpenReadStream());
                                command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = image.FileName;
                                command.Parameters.Add("@PostId", SqlDbType.Int).Value = post_id;
                                var newImageId = Convert.ToInt32(await command.ExecuteScalarAsync());
                            }
                        }

                        transaction.Commit();
                        return Ok("Images uploaded and inserted into the database.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return StatusCode(500, "Error uploading and inserting images.");
                    }
                }
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetImage(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT ImageData FROM Images WHERE ImageID = @ImageID", connection))
                {
                    command.Parameters.Add("@ImageID", SqlDbType.Int).Value = id;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            byte[] imageData = (byte[])reader["ImageData"];
                            return File(imageData, "image/jpeg"); // Adjust the content type as needed
                        }
                    }
                }
            }

            return NotFound("Image not found.");
        }


        private byte[] ReadFully(Stream input)
        {
            using (var memoryStream = new MemoryStream())
            {
                input.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

    }

}
