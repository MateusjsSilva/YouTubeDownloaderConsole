using YoutubeExplode;
using YoutubeExplode.Converter;

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

        // Paths to the ffmpeg, ffplay, and ffprobe files within the storage folder
        string ffmpegPath = Path.Combine(storageFolder, "ffmpeg.exe");
        string ffplayPath = Path.Combine(storageFolder, "ffplay.exe");
        string ffprobePath = Path.Combine(storageFolder, "ffprobe.exe");

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

        // Read URLs from the file
        var urls = File.ReadAllLines(filePath).Where(url => !string.IsNullOrWhiteSpace(url)).ToList();

        if (urls.Count == 0)
        {
            Console.WriteLine("The file does not contain valid URLs.");
            return;
        }

        // Instantiate the YoutubeExplode client
        var youtube = new YoutubeClient();

        foreach (var url in urls)
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

        Console.WriteLine("All videos have been processed.");
    }

    // Method to sanitize the file name, removing invalid characters
    private static string SanitizeFileName(string name)
    {
        return string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
    }
}