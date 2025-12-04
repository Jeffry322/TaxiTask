using System.Data;
using Dapper;
using ETL.Task.Entities;
using ETL.Task.Models;
using Microsoft.Data.SqlClient;

namespace ETL.Task.Database;

using System.Threading.Tasks;

public sealed class RideRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public RideRepository(DbConnectionFactory factory)
    {
        _connectionFactory = factory;
    }

    public async Task<List<Ride>> GetByPickupLocationIdAsync(int pickupLocationId,
        DateTime? rideStart,
        DateTime? rideEnd)
    {
        var sql = """
                  SELECT * FROM dbo.Rides 
                  WHERE PickupLocationId = @pickupLocationId
                  AND (@rideStart IS NULL OR PickupDateTime >= @rideStart)
                  AND (@rideEnd   IS NULL OR PickupDateTime <  @rideEnd);
                  """;
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        var result = await connection.QueryAsync<Ride>(sql,
            new {pickupLocationId, rideStart, rideEnd});
        
        return result.ToList();
    }
    
    public async Task<List<Ride>> GetTop100ByTravelTimeAsync()
    {
        var sql = """
                  SELECT TOP 100 
                  Id,
                  PickupDateTime,
                  DropoffTime,
                  PassengerCount,
                  Distance,
                  StoreAndFwdFlag,
                  PickupLocationId,
                  DropoffLocationId,
                  FareAmount,
                  TipAmount,
                  TravelTimeSeconds
                  FROM dbo.Rides ORDER BY TravelTimeSeconds DESC
                  """;
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        var result = await connection.QueryAsync<Ride>(sql);
        return result.ToList();
    }
    
    public async Task<List<Ride>> GetTop100ByTravelDistanceAsync()
    {
        var sql = """
                  SELECT TOP 100 
                  Id,
                  PickupDateTime,
                  DropoffTime,
                  PassengerCount,
                  Distance,
                  StoreAndFwdFlag,
                  PickupLocationId,
                  DropoffLocationId,
                  FareAmount,
                  TipAmount
                  FROM dbo.Rides ORDER BY Distance DESC
                  """;
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        var result = await connection.QueryAsync<Ride>(sql);
        return result.ToList();
    }
    
    public async Task<LocationAverageTip> GetHighestAverageTipPickupLocationAsync()
    {
        const string sql = @"
            SELECT TOP 1
                PickupLocationId,
                 AVG(TipAmount) AS AverageTip
            FROM dbo.Rides
            GROUP BY PickupLocationId
            ORDER BY AverageTip DESC;";

        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        var result = await connection.QueryFirstOrDefaultAsync<LocationAverageTip>(
            new CommandDefinition(sql));

        if (result == null)
        {
            throw new InvalidOperationException("No tip average found.");
        }
        
        return result;
    }

    public async Task BulkInsertAsync(List<Ride> rides)
    {
        if (rides.Count == 0)
        {
            return;
        }

        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var copy =
            new SqlBulkCopy((SqlConnection)connection);
        copy.DestinationTableName = "dbo.Rides";
        copy.ColumnMappings.Add("Id", "Id");
        copy.ColumnMappings.Add("PickupDateTime", "PickupDateTime");
        copy.ColumnMappings.Add("DropoffTime", "DropoffTime");
        copy.ColumnMappings.Add("PassengerCount", "PassengerCount");
        copy.ColumnMappings.Add("Distance", "Distance");
        copy.ColumnMappings.Add("StoreAndFwdFlag", "StoreAndFwdFlag");
        copy.ColumnMappings.Add("PickupLocationId", "PickupLocationId");
        copy.ColumnMappings.Add("DropoffLocationId", "DropoffLocationId");
        copy.ColumnMappings.Add("FareAmount", "FareAmount");
        copy.ColumnMappings.Add("TipAmount", "TipAmount");

        var table = BuildDataTable(rides);

        await copy.WriteToServerAsync(table);
    }

    public async Task CleanupAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync("DELETE FROM Rides");
    }

    private DataTable BuildDataTable(List<Ride> rides)
    {
        var table = new DataTable();

        table.Columns.Add("Id", typeof(Guid));
        table.Columns.Add("PickupDateTime", typeof(DateTime));
        table.Columns.Add("DropoffTime", typeof(DateTime));
        table.Columns.Add("PassengerCount", typeof(int));
        table.Columns.Add("Distance", typeof(float));
        table.Columns.Add("StoreAndFwdFlag", typeof(string));
        table.Columns.Add("PickupLocationId", typeof(int));
        table.Columns.Add("DropoffLocationId", typeof(int));
        table.Columns.Add("FareAmount", typeof(decimal));
        table.Columns.Add("TipAmount", typeof(decimal));

        foreach (var r in rides)
        {
            table.Rows.Add(
                r.Id,
                r.PickupDateTime,
                r.DropoffTime,
                r.PassengerCount,
                r.Distance,
                r.StoreAndFwdFlag?.ToString(),
                r.PickupLocationId,
                r.DropoffLocationId,
                r.FareAmount,
                r.TipAmount
            );
        }

        return table;
    }
}