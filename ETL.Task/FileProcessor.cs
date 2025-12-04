using ETL.Task.Entities;

namespace ETL.Task;

using System.Threading.Tasks;

public sealed class FileProcessor
{
    private readonly CsvImporter _importer;
    private readonly CsvExporter _exporter;
    private readonly string _readPath;
    private readonly string _writePath;

    public FileProcessor(string readPath, string writePath)
    {
        ArgumentNullException.ThrowIfNull(readPath);
        _importer = new CsvImporter();
        _exporter = new CsvExporter();
        _readPath = readPath;
        _writePath = writePath;
    }

    public async Task<List<Ride>> ProcessAsync()
    {
        var result = await _importer.ImportAsync(_readPath);
        await _exporter.ExportAsync(result.Item2, _writePath);
        return result.Item1;
    }
}