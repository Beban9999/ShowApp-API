using AppApi.Models;
using AppApi.Repository.Contract;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString;
        private IAuthRepository _authRepository;

        public AuthController(IConfiguration configuration, IAuthRepository authRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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

        [HttpPost("activate")]
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


    }
}
