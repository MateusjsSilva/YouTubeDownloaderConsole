using YouTubeDownloaderConsole.WorkerService.Services;

namespace YouTubeDownloaderConsole.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            // Add Worker as a hosted service
            builder.Services.AddHostedService<Worker>();

            // Add services
            builder.Services.AddSingleton<DownloadQueueService>();
            builder.Services.AddSingleton<YoutubeDLSharpService>();
            builder.Services.AddSingleton<YoutubeExplodeService>();

            var host = builder.Build();
            host.Run();
        }
    }
}