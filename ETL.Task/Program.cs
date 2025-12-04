using ETL.Task;
using ETL.Task.Database;

var inputPathArg= args.Length >= 1 ? args[0] : "sample-cab-data.csv";
var duplicatesPathArg= args.Length >= 2 ? args[1] : "duplicates.csv";
if (!File.Exists(inputPathArg))
{
    Console.WriteLine("Input file not found.");
    return;
}
var connectionString =
    "Server=localhost,1433;Database=TaxiRides;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";

var fileProcessor = new FileProcessor(inputPathArg, duplicatesPathArg);
var rides = await fileProcessor.ProcessAsync();
var repository = new RideRepository(new DbConnectionFactory(connectionString));

await repository.CleanupAsync();
await repository.BulkInsertAsync(rides);

var runner = new ConsoleRunner(repository);
await runner.Run();
