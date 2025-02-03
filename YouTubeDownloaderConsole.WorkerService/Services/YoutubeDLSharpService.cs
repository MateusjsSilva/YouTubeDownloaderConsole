using YoutubeDLSharp.Options;
using YoutubeDLSharp;
using YouTubeDownloaderConsole.WorkerService.Models;

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

        public async Task<Result> DownloadVideoAsync(string url, string outputFolder, string cookiesPath)
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
                    return Result.SuccessResult($"Download concluído com yt-dlp!\nSaída: {string.Join(Environment.NewLine, result.Data)}");
                }
                else
                {
                    return Result.FailureResult($"Erro no yt-dlp: {string.Join(Environment.NewLine, result.ErrorOutput)}");
                }
            }
            catch (Exception ex)
            {
                return Result.FailureResult($"Erro ao executar yt-dlp: {ex.Message}");
            }
        }

        public async Task<Result> DownloadPlaylistAsync(string playlistUrl, string outputFolder, string cookiesPath)
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
                    return Result.SuccessResult("Download da playlist concluído com yt-dlp!");
                }
                else
                {
                    return Result.FailureResult($"Erro no yt-dlp: {string.Join(Environment.NewLine, result.ErrorOutput)}");
                }
            }
            catch (Exception ex)
            {
                return Result.FailureResult($"Erro ao executar yt-dlp: {ex.Message}");
            }
        }

        private string SanitizeFileName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).Trim();
        }
    }
}