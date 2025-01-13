using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Converter;

namespace YouTubeDownloaderConsole.Services
{
    public static class DownloadService
    {
        /// <summary>
        /// Downloads a YouTube video and saves it to the specified output folder.
        /// </summary>
        /// <param name="youtube">The YoutubeClient instance used to interact with the YouTube API.</param>
        /// <param name="url">The URL of the YouTube video to download.</param>
        /// <param name="outputFolder">The folder where the downloaded video will be saved.</param>
        /// <param name="ffmpegPath">The path to the ffmpeg executable used for video conversion.</param>
        public static async Task DownloadVideo(YoutubeClient youtube, string url, string outputFolder, string ffmpegPath)
        {
            try
            {
                // Get video information
                var video = await youtube.Videos.GetAsync(url);

                // Path to save the video
                string filePathToSave = Path.Combine(outputFolder, $"{SanitizeFileName(video.Title)}.mp4");

                // Download the video with conversion
                await youtube.Videos.DownloadAsync(url, filePathToSave, o => o
                    .SetContainer("mp4")
                    .SetFFmpegPath(ffmpegPath)
                    .SetPreset(ConversionPreset.UltraFast));

                Console.WriteLine($"Video downloaded: {video.Title}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading video from {url}: {ex.Message}");
            }
        }

        /// <summary>
        /// Downloads all videos from a YouTube playlist and saves them to the specified output folder.
        /// </summary>
        /// <param name="youtube">The YoutubeClient instance used to interact with the YouTube API.</param>
        /// <param name="playlistUrl">The URL of the YouTube playlist to download.</param>
        /// <param name="outputFolder">The folder where the downloaded videos will be saved.</param>
        /// <param name="ffmpegPath">The path to the ffmpeg executable used for video conversion.</param>
        public static async Task DownloadPlaylist(YoutubeClient youtube, string playlistUrl, string outputFolder, string ffmpegPath)
        {
            try
            {
                var playlist = await youtube.Playlists.GetAsync(playlistUrl);
                var videos = await youtube.Playlists.GetVideosAsync(playlist.Id);

                foreach (var video in videos)
                {
                    await DownloadVideo(youtube, video.Url, outputFolder, ffmpegPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading playlist from {playlistUrl}: {ex.Message}");
            }
        }

        /// <summary>
        /// Sanitizes a file name by removing invalid characters.
        /// </summary>
        /// <param name="name">The original file name.</param>
        /// <returns>A sanitized file name with invalid characters replaced by underscores.</returns>
        private static string SanitizeFileName(string name)
        {
            return string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
        }
    }
}