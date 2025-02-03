using YoutubeDLSharp.Options;
using YoutubeDLSharp;
using YouTubeDownloaderConsole.WorkerService.Exceptions;

namespace YouTubeDownloaderConsole.WorkerService.Services
{
    public class YoutubeDLSharpService
    {
        private readonly YoutubeDL _ytdl;

        /// <summary>
        /// Initializes a new instance of the <see cref="YoutubeDLSharpService"/> class.
        /// </summary>
        public YoutubeDLSharpService()
        {
            _ytdl = new YoutubeDL
            {
                YoutubeDLPath = "Dependencies/yt-dlp.exe",
                FFmpegPath = "Dependencies/ffmpeg.exe"
            };
        }

        /// <summary>
        /// Asynchronously downloads a video from the specified URL.
        /// </summary>
        /// <param name="url">The URL of the video to download.</param>
        /// <param name="outputFolder">The output folder where the downloaded file will be saved.</param>
        /// <param name="cookiesPath">The path to the cookies file.</param>
        /// <returns>A task that represents the asynchronous download operation. The task result contains a boolean indicating success or failure.</returns>
        /// <exception cref="WorkerExceptions">Thrown when an error occurs during the download process.</exception>
        public async Task<bool> DownloadVideoAsync(string url, string outputFolder, string cookiesPath)
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

                if (!result.Success)
                    throw new WorkerExceptions($"Error in yt-dlp: {string.Join(Environment.NewLine, result.ErrorOutput)}");

                return true;
            }
            catch (Exception ex)
            {
                throw new WorkerExceptions($"Error running yt-dlp: {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously downloads a playlist from the specified URL.
        /// </summary>
        /// <param name="playlistUrl">The URL of the playlist to download.</param>
        /// <param name="outputFolder">The output folder where the downloaded files will be saved.</param>
        /// <param name="cookiesPath">The path to the cookies file.</param>
        /// <returns>A task that represents the asynchronous download operation. The task result contains a boolean indicating success or failure.</returns>
        /// <exception cref="WorkerExceptions">Thrown when an error occurs during the download process.</exception>
        public async Task<bool> DownloadPlaylistAsync(string playlistUrl, string outputFolder, string cookiesPath)
        {
            try
            {
                var options = new OptionSet
                {
                    Output = Path.Combine(outputFolder, "%(playlist)s/%(title)s.%(ext)s"),
                    Cookies = cookiesPath
                };

                var result = await _ytdl.RunVideoPlaylistDownload(playlistUrl, overrideOptions: options);

                if (!result.Success)
                    throw new WorkerExceptions($"Error in yt-dlp: {string.Join(Environment.NewLine, result.ErrorOutput)}");

                return true;
            }
            catch (Exception ex)
            {
                throw new WorkerExceptions($"Error running yt-dlp: {ex.Message}");
            }
        }
    }
}