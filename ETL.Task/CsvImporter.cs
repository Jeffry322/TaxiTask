using ETL.Task.Entities;
using ETL.Task.Models;

namespace ETL.Task;

using System.Threading.Tasks;

public sealed class CsvImporter
{
    private const int PickupDateIndex = 1;
    private const int DropOffDateIndex = 2;
    private const int PassengerCountIndex = 3;
    private const int TripDistanceIndex = 4;
    private const int StoreAndFwdIndex = 6;
    private const int PuLocationIndex = 7;
    private const int DoLocationIndex = 8;
    private const int FareAmountIndex = 10;
    private const int TipAmountIndex = 13;
    
    private const int ExpectedHeaderCount = 18;
    
    public async Task<(List<Ride>, List<Ride>)> ImportAsync(string filePath)
    {
        using var reader = new StreamReader(filePath);
        _ = await reader.ReadLineAsync();
        
        List<Ride> uniqueRides = [];
        List<Ride> duplicateRides = [];
        HashSet<RideKey> seenKeys = [];
        
        while  (await reader.ReadLineAsync() is { } line)
        {
            var split = line.Split(",");
            if (split.Length != ExpectedHeaderCount) continue;
            
            var pickupDateTime = DateTime.TryParse(split[PickupDateIndex], out var pickupDate) ?
                pickupDate : default;
            
            var dropoffDateTime = DateTime.TryParse(split[DropOffDateIndex], out var dropoffDate) ?
                dropoffDate : default;
            
            int? passengerCount = int.TryParse(split[PassengerCountIndex], out var passengerCountValue) ?
                passengerCountValue : null;
            
            var tripDistance = float.TryParse(split[TripDistanceIndex], out var tripDistanceValue) ?
                tripDistanceValue : default;
            
            var storeAndFwdFlag = ParseStoreAndFwdFlag(split[StoreAndFwdIndex]);
            
            var pickupLocationId = int.TryParse(split[PuLocationIndex], out var pickupLocationIdValue) ?
                pickupLocationIdValue : default;
            
            var dropoffLocationId = int.TryParse(split[DoLocationIndex], out var dropoffLocationIdValue) ?
                dropoffLocationIdValue : default;
            
            var fareAmount = decimal.TryParse(split[FareAmountIndex], out var fareAmountValue) ?
                fareAmountValue : default;
            
            var tipAmount = decimal.TryParse(split[TipAmountIndex], out var tipAmountValue) ?
                tipAmountValue : default;
            
            pickupDateTime = TimeZoneInfo
                .ConvertTimeToUtc(pickupDateTime,
                    TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            dropoffDateTime = TimeZoneInfo                                         
                .ConvertTimeToUtc(dropoffDateTime,                                 
                    TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            
            var ride = new Ride(
                pickupDateTime,
                dropoffDateTime,
                passengerCount,
                tripDistance,
                storeAndFwdFlag,
                pickupLocationId,
                dropoffLocationId,
                fareAmount,
                tipAmount);
            
            var key = new RideKey(ride.PickupDateTime, ride.DropoffTime, ride.PassengerCount);

            if (seenKeys.Add(key))
            {
                uniqueRides.Add(ride);
            }
            else
            {
                duplicateRides.Add(ride);
            }
        }

        Console.WriteLine($"File had {uniqueRides.Count} unique rides and {duplicateRides.Count} duplicate rides.");
        return (uniqueRides, duplicateRides);
    }
    
    private StoreAndFwdFlag? ParseStoreAndFwdFlag(string flag) => flag.Trim() switch
    {
        "N" => StoreAndFwdFlag.No,
        "Y" => StoreAndFwdFlag.Yes,
        _ => null
    };
}