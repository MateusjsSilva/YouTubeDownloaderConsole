namespace YouTubeDownloaderConsole.WorkerService.Models
{
    public class Result
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string Error { get; set; } = string.Empty;

        public static Result SuccessResult(string message)
        {
            return new Result { Success = true, Message = message };
        }

        public static Result FailureResult(string error)
        {
            return new Result { Success = false, Error = error };
        }
    }
}