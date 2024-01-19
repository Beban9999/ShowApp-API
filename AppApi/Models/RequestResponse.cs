namespace AppApi.Models
{
    public class RequestResponse
    {
        public bool IsSuccessfull { get; set; }
        public string? ErrorMessage { get; set; }
        public int Result { get; set; }
    }
}
