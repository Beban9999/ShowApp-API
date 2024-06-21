using System;
using AppApi.Models;
using AppApi.Models.Artist;
using AppApi.Models.Post;
using AppApi.Repository.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
	{
        private IMediaRepository _imageRepository;

        public MediaController(IMediaRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        [HttpPost("upload")]
        public ActionResult<Response> Upload_Media([FromForm] ArtistMediaRequest request)
        {
            Response response = new Response();
            try
            {
                RequestResponse resp = _imageRepository.UploadMedia(request.Files?.Files, request.Id, request.IsProfile, request.PostId);
                if (resp.IsSuccessfull)
                {
                    response.Data = JsonConvert.SerializeObject(true);
                    response.Message = "Media successfully inserted!";
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

    }
}

