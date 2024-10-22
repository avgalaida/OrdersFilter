namespace OrdersFilter.Services;

public interface IFileService
{
    IAsyncEnumerable<string> ReadLinesAsync(string filePath);
    Task WriteLinesAsync(string filePath, IEnumerable<string> lines);
}