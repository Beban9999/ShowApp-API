using AppApi.Helper;
using AppApi.Models;
using AppApi.Repository.Contract;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AppApi.Repository
{
    public class AuthRepository : IAuthRepository
    {
        public DbHelper _dbHelper;
        public AuthRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
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
                else
                {
                    response.IsSuccessfull = false;
                    response.ErrorMessage = "User not registered!";
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

                int resp = _dbHelper.ExecProcReturnScalar(parameters, "usp_active_user");
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


        public RequestResponse LoginUser(LoginRequest loginRequest)
        {
            RequestResponse response = new RequestResponse();

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
                    response.IsSuccessfull = true;
                }
                else
                {
                    response.IsSuccessfull = false;
                    response.ErrorMessage = "Login name or password is invalid!";
                }
            }
            catch(Exception ex)
            {
                response.IsSuccessfull = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
