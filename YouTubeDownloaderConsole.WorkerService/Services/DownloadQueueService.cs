using System.Collections.Concurrent;

namespace YouTubeDownloaderConsole.WorkerService.Services
{
    public class DownloadTask(string url, bool isPlaylist)
    {
        public string Url { get; } = url;

        public bool IsPlaylist { get; } = isPlaylist;
    }

    public class DownloadQueueService
    {
        private readonly ConcurrentQueue<DownloadTask> _queue = new();
        private readonly SemaphoreSlim _signal = new(0);

        /// <summary>
        /// Enqueues a new download task with the specified URL and playlist flag.
        /// </summary>
        /// <param name="url">The URL of the video or playlist to download.</param>
        /// <param name="isPlaylist">Indicates whether the URL is a playlist.</param>
        public void EnqueueDownload(string url, bool isPlaylist)
        {
            _queue.Enqueue(new DownloadTask(url, isPlaylist));
            _signal.Release();
        }

        /// <summary>
        /// Asynchronously retrieves the next download task from the queue.
        /// </summary>
        /// <returns>The next download task, or null if the queue is empty.</returns>
        public async Task<DownloadTask?> GetNextDownloadTaskAsync()
        {
            await _signal.WaitAsync();

            if (_queue.TryDequeue(out var task))
                return task;

            return null;
        }
    }
}