using Microsoft.AspNetCore.Http.HttpResults;

namespace AppApi.Models
{
    public enum RequestStatus
    {
        Error = 0,
        Success = 1,
        InvalidRequest = 2
    }
}
