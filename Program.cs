using YoutubeExplode;
using YoutubeExplode.Converter;

class Program
{
    static async Task Main(string[] args)
    {
        // Solicitar ao usuário que insira o caminho do arquivo de URLs
        Console.WriteLine("Digite o caminho completo do arquivo de texto contendo as URLs do YouTube:");
        string filePath = Console.ReadLine();

        // Verificar se o arquivo existe
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Arquivo não encontrado. Certifique-se de que o caminho está correto.");
            return;
        }

        // Solicitar ao usuário que selecione a pasta de saída
        Console.WriteLine("Digite o caminho completo da pasta onde os vídeos serão salvos:");
        string outputFolder = Console.ReadLine();

        // Verificar se a pasta existe, caso contrário, criar
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        // Ler URLs do arquivo
        var urls = File.ReadAllLines(filePath).Where(url => !string.IsNullOrWhiteSpace(url)).ToList();

        if (urls.Count == 0)
        {
            Console.WriteLine("O arquivo não contém URLs válidas.");
            return;
        }

        // Instanciar o cliente do YoutubeExplode
        var youtube = new YoutubeClient();

        // Processar cada URL e baixar o vídeo
        foreach (var url in urls)
        {
            try
            {
                // Obter informações do vídeo
                var video = await youtube.Videos.GetAsync(url);

                // Caminho para salvar o vídeo
                string filePathToSave = Path.Combine(outputFolder, $"{SanitizeFileName(video.Title)}.mp4");

                // Baixar o vídeo com conversão
                await youtube.Videos.DownloadAsync(url, filePathToSave, o => o
                    .SetContainer("mp4")
                    .SetPreset(ConversionPreset.UltraFast));

                Console.WriteLine($"Vídeo baixado: {video.Title}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao baixar vídeo de {url}: {ex.Message}");
            }
        }

        Console.WriteLine("Todos os vídeos foram processados.");
    }

    // Método para sanitizar o nome do arquivo, removendo caracteres inválidos
    private static string SanitizeFileName(string name)
    {
        return string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
    }
}