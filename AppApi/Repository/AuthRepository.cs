using AppApi.Helper;
using AppApi.Models;
using AppApi.Repository.Contract;
using Microsoft.Data.SqlClient;

namespace AppApi.Repository
{
    public class AuthRepository : IAuthRepository
    {
        public DbHelper _dbHelper;
        public AuthRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
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
            throw new NotImplementedException();
        }

        public RequestResponse LoginUser(LoginRequest loginRequest)
        {
            throw new NotImplementedException();
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
                    //response message
                };

                int resp = _dbHelper.ExecProcReturnScalar(parameters, "usp_AddUser");
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
    }
}
