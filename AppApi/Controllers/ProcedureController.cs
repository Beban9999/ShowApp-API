using AppApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace YourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly string _connectionString;

        public LoginController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("register")]
        public async Task<ActionResult<ProcedureResult>> CallRegisterStoredProcedure([FromBody] RegisterRequest registerRequest)
        {
            string loginName = registerRequest.LoginName;
            string password = registerRequest.Password;
            string fName = registerRequest.FirstName;
            string lname = registerRequest.LastName;
            string email = registerRequest.Email;
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand("usp_AddUser", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@pLogin", loginName));
            command.Parameters.Add(new SqlParameter("@pFirstName", fName));
            command.Parameters.Add(new SqlParameter("@pLastName", lname));
            command.Parameters.Add(new SqlParameter("@Email", email));
            command.Parameters.Add(new SqlParameter("@pPassword", password));
            command.Parameters.Add(new SqlParameter("@responseMessage", SqlDbType.NVarChar, 250)
            {
                Direction = ParameterDirection.Output
            });

            await command.ExecuteNonQueryAsync();

            var responseMessage = command.Parameters["@responseMessage"].Value as string;

            if (responseMessage == null)
            {
                return NotFound();
            }

            var result = new ProcedureResult
            {
                ReturnValue = responseMessage,
                ReturnCode = (responseMessage == "Success") ? 0 : 1
            };

            return result;
        }
        [HttpGet("activateUser")]
        public async Task<ActionResult<string>> ActivateUser([FromQuery] int userId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand("UPDATE Users SET isActive = 1 WHERE UserID = @UserID", connection);
            command.Parameters.Add(new SqlParameter("@UserID", userId));

            int rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                // User is successfully activated
                return "You successfully activated your email. You can exit this page.";
            }
            else
            {
                // No user with the provided userId found
                return NotFound();
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<UserStatusRequest>> CheckUserIsActive(string email)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand("SELECT isActive, UserID FROM Users WHERE Email = @Email", connection);
            command.Parameters.Add(new SqlParameter("@Email", email));

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (reader.Read())
            {
                var isActive = reader.GetBoolean(0);
                var userId = reader.GetInt32(1);

                var userStatus = new UserStatusRequest
                {
                    IsActive = isActive,
                    UserID = userId
                };

                return userStatus;
            }
            else
            {
                // Email not found in the Users table, handle as needed
                return NotFound();
            }
        }


        [HttpPost]
        public async Task<ActionResult<ProcedureResult>> CallLoginStoredProcedure([FromBody] LoginRequest loginRequest)
        {
            string loginName = loginRequest.LoginName;
            string password = loginRequest.Password;
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand("usp_Login", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@pLoginName", loginName));
            command.Parameters.Add(new SqlParameter("@pPassword", password));
            command.Parameters.Add(new SqlParameter("@responseMessage", SqlDbType.NVarChar, 250)
            {
                Direction = ParameterDirection.Output
            });

            await command.ExecuteNonQueryAsync();

            var responseMessage = command.Parameters["@responseMessage"].Value as string;

            if (responseMessage == null)
            {
                return NotFound();
            }

            var result = new ProcedureResult
            {
                ReturnValue = responseMessage,
                ReturnCode = (responseMessage == "User successfully logged in") ? 0 : 1
        };

            return result;
        }
    }
}
