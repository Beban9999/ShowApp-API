namespace AppApi.Models
{
    public class Response
    {
        public RequestStatus Status { get; set; }
        public string? Message { get; set; }
        public string? Data { get; set; }
    }
}
