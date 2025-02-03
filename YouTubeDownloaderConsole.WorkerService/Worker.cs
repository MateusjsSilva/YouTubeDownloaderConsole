using YouTubeDownloaderConsole.WorkerService.Models;
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
            _logger.LogInformation("Worker iniciado e aguardando tarefas...");

            while (!stoppingToken.IsCancellationRequested)
            {
                var task = await _queueService.GetNextDownloadTaskAsync();

                if (task != null)
                {
                    _logger.LogInformation("Iniciando download: {0}", task.Url);
                    string outputFolder = Path.Combine(Environment.CurrentDirectory, "Downloads");
                    string cookiesPath = Path.Combine(Environment.CurrentDirectory, "Files", "cookies.txt");

                    Result result;

                    if (task.IsPlaylist)
                    {
                        result = await _youtubeDLSharpService.DownloadPlaylistAsync(task.Url, outputFolder, cookiesPath);
                    }
                    else
                    {
                        result = await _youtubeDLSharpService.DownloadVideoAsync(task.Url, outputFolder, cookiesPath);
                    }

                    if (result.Success)
                    {
                        _logger.LogInformation(result.Message);
                    }
                    else
                    {
                        _logger.LogError(result.Error);
                    }
                }
            }
        }
    }
}