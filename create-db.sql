
IF DB_ID('TaxiRides') IS NOT NULL
BEGIN
    DROP DATABASE TaxiRides;
END
GO

CREATE DATABASE TaxiRides;
GO

USE TaxiRides;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO


CREATE TABLE dbo.Rides (
    Id               uniqueidentifier NOT NULL,
    PickupDateTime   datetime2        NOT NULL,
    DropoffTime      datetime2        NOT NULL,
    PassengerCount   int              NULL,
    Distance         real             NOT NULL,
    StoreAndFwdFlag  nvarchar(max)    NULL,
    PickupLocationId int              NOT NULL,
    DropoffLocationId int             NOT NULL,
    FareAmount       decimal(18,2)    NOT NULL,
    TipAmount        decimal(18,2)    NOT NULL,
    TravelTimeSeconds AS DATEDIFF(SECOND, PickupDateTime, DropoffTime) PERSISTED,
    CONSTRAINT PK_Rides PRIMARY KEY (Id)
);
GO



CREATE NONCLUSTERED INDEX IX_Rides_PickupLocation_Tip
ON dbo.Rides(PickupLocationId)
INCLUDE (TipAmount);
GO

CREATE NONCLUSTERED INDEX IX_Rides_TripDistance
ON dbo.Rides(Distance DESC);
GO

CREATE NONCLUSTERED INDEX IX_Rides_TravelTimeSeconds
ON dbo.Rides(TravelTimeSeconds DESC);
GO

CREATE NONCLUSTERED INDEX IX_Rides_PickupLocation_Date
ON dbo.Rides(PickupLocationId, PickupDateTime);
GO
