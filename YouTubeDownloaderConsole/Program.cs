using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YouTubeDownloaderConsole.WorkerService;
using YouTubeDownloaderConsole.WorkerService.Services;

class Program
{
    static async Task Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<Worker>();
                services.AddSingleton<DownloadQueueService>();
                services.AddSingleton<YoutubeDLSharpService>();
            })
            .Build();

        await host.StartAsync();

        var queueService = host.Services.GetRequiredService<DownloadQueueService>();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=====================================");
            Console.WriteLine("      YouTube Downloader Console");
            Console.WriteLine("=====================================");
            Console.WriteLine("1. Download a video");
            Console.WriteLine("2. Download a playlist");
            Console.WriteLine("3. Download videos from a file");
            Console.WriteLine("4. Download playlists from a file");
            Console.WriteLine("5. Exit");
            Console.Write("Option: ");

            string? option = Console.ReadLine();
            if (option == "5") break;

            Console.Write("Enter the output directory (or press Enter to use the default 'Downloads/YoutubeDownloader'): ");
            string? outputFolder = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(outputFolder))
            {
                outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "YoutubeDownloader");
                Directory.CreateDirectory(outputFolder);
            }

            switch (option)
            {
                case "1":
                case "2":
                    Console.Write("Enter the URL of the video/playlist: ");
                    string? url = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(url))
                    {
                        Console.WriteLine("Invalid URL. Press Enter to continue...");
                        Console.ReadLine();
                        continue;
                    }

                    bool isPlaylist = option == "2";
                    queueService.EnqueueDownload(url, isPlaylist, outputFolder);
                    Console.WriteLine("Download added to the queue! Press Enter to continue...");
                    Console.ReadLine();
                    break;

                case "3":
                    Console.Write("Enter the file name (or press Enter to use 'urls.txt'): ");
                    string? videoFile = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(videoFile))
                    {
                        videoFile = "Files/urls.txt";
                    }
                    EnqueueLinksFromFile(queueService, videoFile, false, outputFolder);
                    Console.WriteLine("Video downloads added to the queue! Press Enter to continue...");
                    Console.ReadLine();
                    break;

                case "4":
                    Console.Write("Enter the file name (or press Enter to use 'playlists.txt'): ");
                    string? playlistFile = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(playlistFile))
                    {
                        playlistFile = "Files/playlists.txt";
                    }
                    EnqueueLinksFromFile(queueService, playlistFile, true, outputFolder);
                    Console.WriteLine("Playlist downloads added to the queue! Press Enter to continue...");
                    Console.ReadLine();
                    break;

                default:
                    Console.WriteLine("Invalid option. Press Enter to continue...");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private static void EnqueueLinksFromFile(DownloadQueueService queueService, string filePath, bool isPlaylist, string outputFolder)
    {
        if (File.Exists(filePath))
        {
            var urls = File.ReadAllLines(filePath);
            foreach (var url in urls)
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    queueService.EnqueueDownload(url, isPlaylist, outputFolder);
                }
            }
        }
        else
        {
            Console.WriteLine($"File {filePath} not found. Press Enter to continue...");
            Console.ReadLine();
        }
    }
}