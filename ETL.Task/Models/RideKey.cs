namespace ETL.Task.Models;

public record RideKey(DateTime PickupDateTime, DateTime DropoffDateTime, int? PassengerCount);