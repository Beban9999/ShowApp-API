using AppApi.Models;

namespace AppApi.Repository.Contract
{
    public interface IAuthRepository
    {
        public RequestResponse RegisterUser(RegisterRequest registerRequest);
        public RequestResponse ActivateUser(int userId);
        public UserStatusRequest CheckUserIsActive(string email);
        public Response LoginUser(LoginRequest loginRequest);    
        public UserData GetUser(string username);
        public Artist GetArtistWithData(int artistId);
    }
}
