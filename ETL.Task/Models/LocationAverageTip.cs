namespace ETL.Task.Models;

public sealed class LocationAverageTip
{
    public int PickupLocationId { get; private set; }
    public decimal AverageTip { get; private set; }

    public LocationAverageTip(int pickupLocationId, decimal averageTip)
    {
        PickupLocationId = pickupLocationId;
        AverageTip = averageTip;
    }

    public LocationAverageTip()
    {
        
    }
}