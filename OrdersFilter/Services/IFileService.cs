namespace OrdersFilter.Services;

public interface IFileService
{
    Task<IEnumerable<string>> ReadLinesAsync(string filePath);
    Task WriteLinesAsync(string filePath, IEnumerable<string> lines);
}