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
                services.AddSingleton<YoutubeExplodeService>();
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
            Console.WriteLine("1. Baixar um vídeo");
            Console.WriteLine("2. Baixar uma playlist");
            Console.WriteLine("3. Sair");
            Console.Write("Opção: ");

            string option = Console.ReadLine();
            if (option == "3") break;

            Console.Write("Digite a URL do vídeo/playlist: ");
            string url = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(url))
            {
                Console.WriteLine("URL inválida. Pressione Enter para continuar...");
                Console.ReadLine();
                continue;
            }

            bool isPlaylist = option == "2";
            queueService.EnqueueDownload(url, isPlaylist);
            Console.WriteLine("Download adicionado à fila! Pressione Enter para continuar...");
            Console.ReadLine();
        }
    }
}