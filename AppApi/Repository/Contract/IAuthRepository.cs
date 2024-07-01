using AppApi.Models;
using AppApi.Models.Artist;

namespace AppApi.Repository.Contract
{
    public interface IAuthRepository
    {
        public RequestResponse RegisterUser(RegisterRequest registerRequest);
        public RequestResponse ActivateUser(int userId);
        public UserStatusRequest CheckUserIsActive(string email);
        public Response LoginUser(LoginRequest loginRequest);    
        public UserData GetUser(string username);
        public RequestResponse ValidateField(string fieldName, string fieldValue);
        public RequestResponse CheckPassword(int userId, string password);
    }
}
