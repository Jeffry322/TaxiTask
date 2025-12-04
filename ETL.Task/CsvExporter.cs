using System.Globalization;

namespace ETL.Task;

using System.Threading.Tasks;
using Entities;

public sealed class CsvExporter
{
    public async Task ExportAsync(IEnumerable<Ride> rides, string path)
    {
        await using var writer = new StreamWriter(path);

        await writer.WriteLineAsync(
            "tpep_pickup_datetime," +
            "tpep_dropoff_datetime," +
            "passenger_count," +
            "trip_distance," +
            "store_and_fwd_flag," +
            "PULocationID," +
            "DOLocationID," +
            "fare_amount," +
            "tip_amount");
        
        foreach (var ride in rides)
        {
            var line = string.Join(",",
                ride.PickupDateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                ride.DropoffTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                ride.PassengerCount?.ToString(CultureInfo.InvariantCulture) ?? "",
                ride.Distance.ToString(CultureInfo.InvariantCulture),
                ride.StoreAndFwdFlag.ToString() ?? "",
                ride.PickupLocationId.ToString(CultureInfo.InvariantCulture),
                ride.DropoffLocationId.ToString(CultureInfo.InvariantCulture),
                ride.FareAmount.ToString(CultureInfo.InvariantCulture),
                ride.TipAmount.ToString(CultureInfo.InvariantCulture)
            );

            await writer.WriteLineAsync(line);
        }
    }
}