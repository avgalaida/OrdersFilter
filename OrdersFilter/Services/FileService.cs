namespace OrdersFilter.Services;

public class FileService : IFileService
{
    public async IAsyncEnumerable<string> ReadLinesAsync(string filePath)
    {
        using var reader = new StreamReader(filePath);
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            yield return line;
        }
    }

    public async Task WriteLinesAsync(string filePath, IEnumerable<string> lines)
    {
        using var writer = new StreamWriter(filePath, append: true);
        foreach (var line in lines)
        {
            await writer.WriteLineAsync(line);
        }
    }
}