using ETL.Task.Database;

namespace ETL.Task;

using System.Threading.Tasks;

public sealed class ConsoleRunner
{
    private readonly RideRepository _repository;

    public ConsoleRunner(RideRepository repository)
    {
        _repository = repository;
    }

    public async Task Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("==== TaxiRides Console ====");
            Console.WriteLine("1) Show pickup location with highest average tip");
            Console.WriteLine("2) Show top 100 rides by distance");
            Console.WriteLine("3) Show top 100 rides by travel time");
            Console.WriteLine("4) Search rides by pickup location and optional date range");
            Console.WriteLine("5) Exit");
            Console.Write("Choose option: ");

            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await GetHighestAverageTipPickupLocationAsync();
                        break;

                    case "2":
                        await ShowTopByDistanceAsync();
                        break;

                    case "3":
                        await ShowTopByTravelTimeAsync();
                        break;

                    case "4":
                        await SearchByPickupLocationAsync();
                        break;

                    case "5":
                        return;

                    default:
                        Console.WriteLine("Unknown option.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private async Task ShowTopByTravelTimeAsync()
    {
        var top100 = await _repository.GetTop100ByTravelTimeAsync();
        Console.WriteLine("TravelId\tPickupPoint\tDropoffPoint\tDistanceTraveled");
        foreach (var ride in top100)
        {
            Console.WriteLine($"{ride.Id}\t{ride.PickupLocationId}\t{ride.DropoffLocationId}\t{ride.TravelTimeSeconds} seconds");
        }
    }
    
    private async Task ShowTopByDistanceAsync()
    {
        var top100 = await _repository.GetTop100ByTravelDistanceAsync();
        Console.WriteLine("TravelId\tPickupPoint\tDropoffPoint\tDistanceTraveled");
        foreach (var ride in top100)
        {
            Console.WriteLine($"{ride.Id}\t{ride.PickupLocationId}\t{ride.DropoffLocationId}\t{ride.Distance}");
        }
    }
    
    private async Task GetHighestAverageTipPickupLocationAsync()
    {
        var highesAvg = (await _repository.GetHighestAverageTipPickupLocationAsync());
        Console.WriteLine($"{highesAvg.AverageTip}\t{highesAvg.PickupLocationId}");
    }
    
    private async Task SearchByPickupLocationAsync()
    {
        Console.Write("Enter pickup location ID: ");
        var pickupLocationId = int.TryParse(Console.ReadLine(), out var pickupLocationIdValue) ? pickupLocationIdValue : default;
        Console.Write("Enter optional start date (yyyy-MM-dd): ");
        DateTime? startDate = DateTime.TryParse(Console.ReadLine(), out var date) ? date : null;
        Console.Write("Enter optional end date (yyyy-MM-dd): ");
        DateTime? endDate = DateTime.TryParse(Console.ReadLine(), out var date2) ? date2 : null;
        
        var search = await _repository.GetByPickupLocationIdAsync(pickupLocationId, startDate, endDate);
        if(search.Count == 0) Console.WriteLine("No rides found.");
        if(startDate is null) Console.WriteLine("Couldn't read start date.");
        if(endDate is null) Console.WriteLine("Couldn't read end date.");
        foreach (var ride in search)
        {
            Console.WriteLine($"LocationId\tRideId\tPickup\tDropoff");
            Console.WriteLine($"{ride.PickupLocationId}\t{ride.Id}\t{ride.PickupDateTime}\t{ride.DropoffTime}");
        }
    }
}