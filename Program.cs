using YouTubeDownloaderConsole.Services;
using YoutubeExplode;
using YoutubeExplode.Common;

class Program
{
    static async Task Main()
    {
        // Create a folder to store files
        string storageFolder = Path.Combine(Environment.CurrentDirectory, "Files");
        if (!Directory.Exists(storageFolder))
        {
            Directory.CreateDirectory(storageFolder);
        }

        string filePath = Path.Combine(storageFolder, "urls.txt");
        string playlistFilePath = Path.Combine(storageFolder, "playlists.txt");

        Console.WriteLine($"Enter the full path of the text file containing YouTube URLs (or press Enter to use the default)");
        Console.WriteLine($"Default: {filePath}):");

        // Read the input file path from the user
        string? inputFilePath = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(inputFilePath))
        {
            filePath = inputFilePath;
        }

        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found. Make sure the path is correct.");
            return;
        }

        Console.WriteLine($"Enter the full path of the text file containing YouTube Playlist URLs (or press Enter to use the default)");
        Console.WriteLine($"Default: {playlistFilePath}):");

        // Read the input playlist file path from the user
        string? inputPlaylistFilePath = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(inputPlaylistFilePath))
        {
            playlistFilePath = inputPlaylistFilePath;
        }

        if (!File.Exists(playlistFilePath))
        {
            Console.WriteLine("Playlist file not found. Make sure the path is correct.");
            return;
        }

        // Paths to the ffmpeg, ffplay, and ffprobe files within the storage folder
        string dependencyPath = Path.Combine(Environment.CurrentDirectory, "Dependencies");
        string ffmpegPath = Path.Combine(dependencyPath, "ffmpeg.exe");
        string ffplayPath = Path.Combine(dependencyPath, "ffplay.exe");
        string ffprobePath = Path.Combine(dependencyPath, "ffprobe.exe");

        if (!File.Exists(ffmpegPath) || !File.Exists(ffplayPath) || !File.Exists(ffprobePath))
        {
            Console.WriteLine("ffmpeg, ffplay, or ffprobe files not found in the storage folder.");
            return;
        }

        // Prompt the user to select the output folder
        Console.WriteLine("Enter the full path of the folder where the videos will be saved:");
        string? outputFolder = Console.ReadLine();

        // Check if the folder exists, otherwise create it
        if (string.IsNullOrWhiteSpace(outputFolder))
        {
            Console.WriteLine("Invalid output folder path.");
            return;
        }

        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        Console.WriteLine($"Files will be saved to: {outputFolder}");

        // Ask the user what they want to download
        Console.WriteLine("What would you like to download?");
        Console.WriteLine("1. Individual videos");
        Console.WriteLine("2. Playlists");
        Console.WriteLine("3. Both");
        string? choice = Console.ReadLine();

        var youtube = new YoutubeClient();

        if (choice == "1" || choice == "3")
        {
            // Read URLs from the file
            var urls = File.ReadAllLines(filePath).Where(url => !string.IsNullOrWhiteSpace(url)).ToList();

            if (urls.Count == 0)
            {
                Console.WriteLine("The file does not contain valid URLs.");
            }
            else
            {
                foreach (var url in urls)
                {
                    Console.WriteLine($"Downloading video from {url}...");
                    await DownloadService.DownloadVideo(youtube, url, outputFolder, ffmpegPath);
                }
            }
        }

        if (choice == "2" || choice == "3")
        {
            // Read playlist URLs from the file
            var playlistUrls = File.ReadAllLines(playlistFilePath).Where(url => !string.IsNullOrWhiteSpace(url)).ToList();

            if (playlistUrls.Count == 0)
            {
                Console.WriteLine("The file does not contain valid playlist URLs.");
            }
            else
            {
                foreach (var playlistUrl in playlistUrls)
                {
                    Console.WriteLine($"Downloading playlist from {playlistUrl}...");
                    await DownloadService.DownloadPlaylist(youtube, playlistUrl, outputFolder, ffmpegPath);
                }
            }
        }

        Console.WriteLine("All videos have been processed.");
    }
}