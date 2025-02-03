using YouTubeDownloaderConsole.WorkerService.Services;

namespace YouTubeDownloaderConsole.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly YoutubeDLSharpService _youtubeDLSharpService;
        private readonly DownloadQueueService _queueService;

        public Worker(ILogger<Worker> logger, YoutubeDLSharpService youtubeDLSharpService, DownloadQueueService queueService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _youtubeDLSharpService = youtubeDLSharpService ?? throw new ArgumentNullException(nameof(youtubeDLSharpService));
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var task = await _queueService.GetNextDownloadTaskAsync();

                if (task != null)
                {
                    string cookiesPath = Path.Combine(Environment.CurrentDirectory, "Files", "cookies.txt");

                    if (task.IsPlaylist)
                    {
                        await _youtubeDLSharpService.DownloadPlaylistAsync(task.Url, task.OutputFolder, cookiesPath);
                    }
                    else
                    {
                        await _youtubeDLSharpService.DownloadVideoAsync(task.Url, task.OutputFolder, cookiesPath);
                    }
                }
                else
                {
                    break;
                }

                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}