namespace ETL.Task.Entities;

public sealed class Ride
{
    public Guid Id { get; private set; }
    public DateTime PickupDateTime { get; private set; }
    public DateTime DropoffTime { get; private set; }
    public int? PassengerCount { get; private set; }
    public float Distance { get; private set; }
    public StoreAndFwdFlag? StoreAndFwdFlag { get; private set; }
    public int PickupLocationId { get; private set; }
    public int DropoffLocationId { get; private set; }
    public decimal FareAmount { get; private set; }
    public decimal TipAmount { get; private set; }
    public int TravelTimeSeconds { get; private set; }

    public Ride() { }
    
    public Ride(
        DateTime pickupDateTime,
        DateTime dropoffTime,
        int? passengerCout,
        float distance,
        StoreAndFwdFlag? storeAndFwdFlag,
        int pickupLocationId,
        int dropoffLocationId,
        decimal fareAmount,
        decimal tipAmount)
    {
        Id = Guid.NewGuid();
        PickupDateTime = pickupDateTime;
        DropoffTime = dropoffTime;
        PassengerCount = passengerCout;
        Distance = distance;
        StoreAndFwdFlag = storeAndFwdFlag;
        PickupLocationId = pickupLocationId;
        DropoffLocationId = dropoffLocationId;
        FareAmount = fareAmount;
        TipAmount = tipAmount;
    }
}

public enum StoreAndFwdFlag
{
    No = 0,
    Yes = 1
}