using AppApi.Models;
using AppApi.Repository;
using AppApi.Repository.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace AppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly string _connectionString;
        private IImageRepository _imageRepository;

        public ImagesController(IConfiguration configuration, IImageRepository imageRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _imageRepository = imageRepository;
        }

        #region old calls

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
        #endregion

        #region new calls
        //new calls
        [HttpPost("upload")]
        public ActionResult<Response> Upload_Image(List<IFormFile> images, int post_id)
        {
            Response response = new Response();
            try
            {
                //New call
                //RequestResponse resp = _imageRepository.UploadMedia(images, post_id);
                RequestResponse resp = _imageRepository.UploadImage(images, post_id);
                if (resp.IsSuccessfull)
                {
                    response.Data = JsonConvert.SerializeObject(true);
                    response.Message = "Images successfully inserted!";
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
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = RequestStatus.Error;
                return BadRequest(response);
            }
        }

        [HttpGet("get")]
        public ActionResult<Response> Get_Image(int id)
        {
            Response response = new Response();
            try
            {
                byte[] image = _imageRepository.Get_Image(id);
                response.Data = JsonConvert.SerializeObject(image);
                response.Status = RequestStatus.Success;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = RequestStatus.Error;
                return BadRequest(response);
            }
        }

        #endregion

    }

}
