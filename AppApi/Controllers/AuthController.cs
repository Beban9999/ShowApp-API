using AppApi.Models;
using AppApi.Models.Artist;
using AppApi.Repository.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("register")]
        public ActionResult<Response> RegisterUser([FromBody] RegisterRequest registerRequest)
        {
            Response response = new Response();
            try
            {
                RequestResponse resp = _authRepository.RegisterUser(registerRequest);
                if (resp.IsSuccessfull)
                {
                    response.Data = JsonConvert.SerializeObject(true);
                    response.Message = "User successfully registered!";
                    response.Status = RequestStatus.Success;
                    return Ok(response);
                }
                else
                {
                    response.Data = JsonConvert.SerializeObject(false);
                    response.Message = resp.ErrorMessage;
                    response.Status = RequestStatus.Error;
                    return Ok(response);
                }
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Status = RequestStatus.Error;
                return BadRequest(response);
            }
        }

        [HttpGet("activate")]
        public ActionResult<Response> ActivateUser(int userId)
        {
            Response response = new Response();
            try
            {
                RequestResponse resp = _authRepository.ActivateUser(userId);
                if (resp.IsSuccessfull)
                {
                    response.Data = JsonConvert.SerializeObject(true);
                    response.Message = "You successfully activated your email. You can exit this page.";
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

        [HttpGet("isActive")]
        public ActionResult<Response> CheckUserIsActive(string email)
        {
            Response response = new Response();
            try
            {
                UserStatusRequest user = _authRepository.CheckUserIsActive(email);
                response.Data = JsonConvert.SerializeObject(user);
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

        [HttpPost("login")]
        public ActionResult<Response> LoginUser([FromBody] LoginRequest loginRequest)
        {
            Response response = new Response();
            try
            {
                Response resp = _authRepository.LoginUser(loginRequest);
                if (resp.Status == RequestStatus.Success)
                {
                    response.Data = JsonConvert.SerializeObject(resp.Data);
                    response.Message = "Login is successfull!";
                    response.Status = RequestStatus.Success;
                    return Ok(response);
                }
                else
                {
                    response.Data = JsonConvert.SerializeObject(false);
                    response.Message = resp.Message;
                    response.Status = RequestStatus.Error;
                    return Ok(response);
                }
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Status = RequestStatus.Error;
                return BadRequest(response);
            }
        }

        [Authorize]
        [HttpGet("user")]
        public ActionResult<Response> GetUser(string username)
        {
            Response response = new Response();
            try
            {
                UserData resp = _authRepository.GetUser(username);
                response.Data = JsonConvert.SerializeObject(resp);
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

    }
}
