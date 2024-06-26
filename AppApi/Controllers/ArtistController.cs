using System;
using AppApi.Models;
using AppApi.Models.Artist;
using AppApi.Models.Post;
using AppApi.Repository;
using AppApi.Repository.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private IArtistRepository _artistRepository;

        public ArtistController(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        [HttpGet("getartists")]
        public ActionResult<Response> GetArtists(int? userId)
        {
            Response response = new Response();
            try
            {
                List<Artist> list = _artistRepository.GetArtists(userId);
                response.Data = JsonConvert.SerializeObject(list);
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

        [HttpGet("gettypes")]
        public ActionResult<Response> GetArtistTypes()
        {
            Response response = new Response();
            try
            {
                List<ArtistType> types = _artistRepository.GetArtistTypes();
                response.Data = JsonConvert.SerializeObject(types);
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

        [HttpGet("getgenres")]
        public ActionResult<Response> GetArtistGenres()
        {
            Response response = new Response();
            try
            {
                List<ArtistGenre> types = _artistRepository.GetArtistGenres();
                response.Data = JsonConvert.SerializeObject(types);
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

        [Authorize]
        [HttpPost("become")]
        public ActionResult<Response> BecomeArtist([FromBody] ArtistRequest request)
        {
            Response response = new Response();
            try
            {
                RequestResponse resp = _artistRepository.BecomeArtist(request);
                if (resp.IsSuccessfull)
                {
                    response.Data = JsonConvert.SerializeObject(true);
                    response.Message = "Artist successfully created!";
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

        [Authorize]
        [HttpPost("insertpost")]
        public ActionResult<Response> InsertPost([FromBody] ArtistPostRequest request)
        {
            Response response = new Response();
            try
            {
                RequestResponse resp = _artistRepository.InsertPost(request);
                if (resp.IsSuccessfull)
                {
                    response.Data = resp.Result.ToString();
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
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = RequestStatus.Error;
                return BadRequest(response);
            }
        }

        [Authorize]
        [HttpPost("removepost")]
        public ActionResult<Response> RemovePost([FromBody] IdRequest request)
        {
            Response response = new Response();
            try
            {
                RequestResponse resp = _artistRepository.RemovePost(request.Id);
                if (resp.IsSuccessfull)
                {
                    response.Data = JsonConvert.SerializeObject(true);
                    response.Message = "Post successfully removed!";
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

        [Authorize]
        [HttpPost("removeartist")]
        public ActionResult<Response> RemoveArtist([FromBody] IdRequest request)
        {
            Response response = new Response();
            try
            {
                RequestResponse resp = _artistRepository.RemoveArtist(request.Id);
                if (resp.IsSuccessfull)
                {
                    response.Data = JsonConvert.SerializeObject(true);
                    response.Message = "Artist successfully removed!";
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
