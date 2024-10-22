namespace OrdersFilter.Services;

public class FileService : IFileService
{
    public async Task<IEnumerable<string>> ReadLinesAsync(string filePath)
    {
        var lines = new List<string>();
        using (var reader = new StreamReader(filePath))
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                lines.Add(line);
            }
        }

        return lines;
    }

    public async Task WriteLinesAsync(string filePath, IEnumerable<string> lines)
    {
        using (var writer = new StreamWriter(filePath, append: true))
        {
            foreach (var line in lines)
            {
                await writer.WriteLineAsync(line);
            }
        }
    }
}