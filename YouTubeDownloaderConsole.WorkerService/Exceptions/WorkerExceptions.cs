namespace YouTubeDownloaderConsole.WorkerService.Exceptions
{
    public class WorkerExceptions : Exception
    {
        public WorkerExceptions() : base() { }

        public WorkerExceptions(string message) : base(message) { }
    }
}