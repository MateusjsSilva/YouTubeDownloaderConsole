using YoutubeDLSharp.Options;
using YoutubeDLSharp;

namespace YouTubeDownloaderConsole.WorkerService.Services
{
    public class YoutubeDLSharpService
    {
        private readonly YoutubeDL _ytdl;

        public YoutubeDLSharpService()
        {
            _ytdl = new YoutubeDL
            {
                YoutubeDLPath = "Dependencies/yt-dlp.exe",
                FFmpegPath = "Dependencies/ffmpeg.exe"
            };
        }

        public async Task DownloadVideoAsync(string url, string outputFolder, string cookiesPath)
        {
            try
            {
                var options = new OptionSet
                {
                    Output = Path.Combine(outputFolder, "%(title)s.%(ext)s"),
                    Cookies = cookiesPath,
                    MergeOutputFormat = DownloadMergeFormat.Mp4
                };

                var result = await _ytdl.RunVideoDownload(url, overrideOptions: options);

                if (result.Success)
                {
                    Console.WriteLine("Download concluído com yt-dlp!");
                    Console.WriteLine($"Saída: {string.Join(Environment.NewLine, result.Data)}");
                }
                else
                {
                    Console.WriteLine($"Erro no yt-dlp: {string.Join(Environment.NewLine, result.ErrorOutput)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao executar yt-dlp: {ex.Message}");
            }
        }

        public async Task DownloadPlaylistAsync(string playlistUrl, string outputFolder, string cookiesPath)
        {
            try
            {
                var options = new OptionSet
                {
                    Output = Path.Combine(outputFolder, "%(playlist)s/%(title)s.%(ext)s"),
                    Cookies = cookiesPath
                };

                var result = await _ytdl.RunVideoPlaylistDownload(playlistUrl, overrideOptions: options);

                if (result.Success)
                {
                    Console.WriteLine("Download da playlist concluído com yt-dlp!");
                }
                else
                {
                    Console.WriteLine($"Erro no yt-dlp: {string.Join(Environment.NewLine, result.ErrorOutput)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao executar yt-dlp: {ex.Message}");
            }
        }

        private string SanitizeFileName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).Trim();
        }
    }
}