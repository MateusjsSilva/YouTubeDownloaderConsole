using System.Collections.Concurrent;

namespace YouTubeDownloaderConsole.WorkerService.Services
{
    public record DownloadTask(string Url, bool IsPlaylist);

    public class DownloadQueueService
    {
        private readonly ConcurrentQueue<DownloadTask> _queue = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void EnqueueDownload(string url, bool isPlaylist)
        {
            _queue.Enqueue(new DownloadTask(url, isPlaylist));
            _signal.Release();
        }

        public async Task<DownloadTask?> GetNextDownloadTaskAsync()
        {
            await _signal.WaitAsync();

            if (_queue.TryDequeue(out var task))
            {
                return task;
            }

            return null;
        }
    }
}