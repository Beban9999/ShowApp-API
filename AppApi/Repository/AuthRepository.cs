using AppApi.Helper;
using AppApi.Models;
using AppApi.Repository.Contract;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AppApi.Repository
{
    public class AuthRepository : IAuthRepository
    {
        public DbHelper _dbHelper;
        private IConfiguration _configuration;
        public AuthRepository(DbHelper dbHelper, IConfiguration configuration)
        {
            _dbHelper = dbHelper;
            _configuration = configuration;
        }

        public RequestResponse RegisterUser(RegisterRequest registerRequest)
        {
            RequestResponse response = new RequestResponse();

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@pLogin", registerRequest.LoginName),
                    new SqlParameter("@pFirstName", registerRequest.FirstName),
                    new SqlParameter("@pLastName", registerRequest.LastName),
                    new SqlParameter("@Email", registerRequest.Email),
                    new SqlParameter("@pPassword", registerRequest.Password)
                };

                int resp = _dbHelper.ExecProcReturnScalar(parameters, "dbo.usp_AddUser");
                if(resp == 1) 
                {
                    response.IsSuccessfull = true;
                }
                else if(resp == 2)
                {
                    response.IsSuccessfull = false;
                    response.ErrorMessage = "Email already exists!";
                }
                else if (resp == 3)
                {
                    response.IsSuccessfull = false;
                    response.ErrorMessage = "Username already exists!";
                }
                else
                {
                    response.IsSuccessfull = false;
                    response.ErrorMessage = "Registration failed";
                }

            }
            catch (Exception ex)
            {
                response.IsSuccessfull = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public RequestResponse ActivateUser(int userId)
        {
            RequestResponse response = new RequestResponse();

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@UserID", userId)
                };

                int resp = _dbHelper.ExecProcReturnScalar(parameters, "usp_ActivateUser");
                if (resp == 1)
                {
                    response.IsSuccessfull = true;
                }
                else
                {
                    response.IsSuccessfull = false;
                    response.ErrorMessage = "User email is not activated!";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccessfull = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public UserData GetUser(string username)
        {
            UserData userData = new UserData();

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("LoginName", username)
            };

            DataTable dt = _dbHelper.ExecProc(parameters, "usp_GetUser");
            if (dt.Rows.Count > 0)
            {
                userData.UserId = DbTypeHelper.GetInt(dt.Rows[0], "UserId");
                userData.LoginName = DbTypeHelper.GetString(dt.Rows[0], "LoginName");
                userData.FirstName = DbTypeHelper.GetString(dt.Rows[0], "FirstName");
                userData.LastName = DbTypeHelper.GetString(dt.Rows[0], "LastName");
                userData.Email = DbTypeHelper.GetString(dt.Rows[0], "Email");
            }

            return userData;
        }

        public UserStatusRequest CheckUserIsActive(string email)
        {
            UserStatusRequest userStatusRequest = new UserStatusRequest();

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("email", email)
            };

            DataTable dt = _dbHelper.ExecProc(parameters, "usp_CheckUser");
            if (dt.Rows.Count > 0)
            {
                userStatusRequest.UserID = DbTypeHelper.GetInt(dt.Rows[0], "UserId");
                userStatusRequest.IsActive = DbTypeHelper.GetBool(dt.Rows[0], "IsActive");
            }

            return userStatusRequest;
        }

        public Response LoginUser(LoginRequest loginRequest)
        {
            Response response = new Response();

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@pLoginName", loginRequest.LoginName),
                    new SqlParameter("@pPassword", loginRequest.Password)
                };

                int resp = _dbHelper.ExecProcReturnScalar(parameters, "usp_Login");
                if(resp == 1)
                {
                    string token = GenerateJwtToken(loginRequest.LoginName);
                    response.Data = token;
                    response.Status = RequestStatus.Success;
                }
                else if(resp == 0)
                {
                    response.Status = RequestStatus.Error;
                    response.Message = "Login name or password is invalid!";
                }
                else 
                {
                    response.Status = RequestStatus.Error;
                    response.Message = "Activate";
                }
            }
            catch(Exception ex)
            {
                response.Status = RequestStatus.Error;
                response.Message = ex.Message;
            }

            return response;
        }



        #region JwtHelper
        private string GenerateJwtToken(string loginName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, loginName),
            };

            var Sectoken = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);
            return token;
        }

        // Used one time to generate the jwt secret key, not inside the appsetting
        private string GenerateRandomKey(int length)
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] keyBytes = new byte[length];
                rng.GetBytes(keyBytes);
                return Convert.ToBase64String(keyBytes);
            }
        }

        #endregion
    }
}
