namespace ArtAssetManager.Api.Errors
{
    public class ApiErrorResponse
    {
        public System.Net.HttpStatusCode Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public ApiErrorResponse(System.Net.HttpStatusCode status, string msg, string path)
        {
            Status = status;
            Message = msg;
            Path = path;
        }

    }


}