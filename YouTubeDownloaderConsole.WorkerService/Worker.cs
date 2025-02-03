using YouTubeDownloaderConsole.WorkerService.Services;

namespace YouTubeDownloaderConsole.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly YoutubeDLSharpService _youtubeDLSharpService;
        private readonly YoutubeExplodeService _youtubeExplodeService;
        private readonly DownloadQueueService _queueService;

        public Worker(ILogger<Worker> logger, YoutubeDLSharpService youtubeDLSharpService, YoutubeExplodeService youtubeExplodeService, DownloadQueueService queueService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _youtubeDLSharpService = youtubeDLSharpService ?? throw new ArgumentNullException(nameof(youtubeDLSharpService));
            _youtubeExplodeService = youtubeExplodeService ?? throw new ArgumentNullException(nameof(youtubeExplodeService));
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
                    string outputFolder = Path.Combine(Environment.CurrentDirectory, "Dependencies");
                    string ffmpegPath = Path.Combine(Environment.CurrentDirectory, "Dependencies", "ffmpeg.exe");
                    string cookiesPath = Path.Combine(Environment.CurrentDirectory, "Files", "cookies.txt");

                    if (task.IsPlaylist)
                    {
                        await _youtubeDLSharpService.DownloadPlaylistAsync(task.Url, outputFolder, cookiesPath);
                    }
                    else
                    {
                        await _youtubeExplodeService.DownloadVideoAsync(task.Url, outputFolder, ffmpegPath);
                    }

                    _logger.LogInformation("Download concluído para: {0}", task.Url);
                }
                else
                {
                    _logger.LogInformation("Nenhuma tarefa na fila. Encerrando Worker...");
                    await Task.Delay(2000, stoppingToken);
                    Environment.Exit(0);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}