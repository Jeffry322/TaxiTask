### Running this program ###
1. Clone the repo
   ```
   git clone https://github.com/Jeffry322/TaxiTask.git
   ```
2. cd to solution root folder
   ```
   cd TaxiTask
   ```
3. Run docker compose with
   ```
   docker compose up --build
   ```
   which will build all containers and init the database
4. Start the project in by running dotnet run
   ```
   dotnet run --project ETL.Task/ETL.Task.csproj
   ```
   you can specify input file by arguments, like
   ```
   dotnet run --project ETL.Task/ETL.Task.csproj -- /path/to/file.csv /path/to/duplicates
   ```
   or the program will pickup file sample-cab-data.csv by itself if it's in project root
5. Teardown
   ```
   docker compose down -v
   ```

### Additonal info ###
1. CSV Has 29889 unique records, and 111 duplicates
2. If I would have much larger datasets I would avoid loading data in memory. Firstly I would break the incoming file in batches, and BulkCopy batch by batch. In Memory dedupe also doesn't scale very well, so I maybe would use database as staging.
3. About security: I TryParse every item from csv, so on bad data I just write default values, we could also just skip the row entirely in TryParse boyd, and log it. Also I used Dapper, and Dapper's parameters aren't vulnerable to SQL injections as string interpolation.
