-- Availability & Pricing Service Database Schema - SQL Server

-- Create database
CREATE DATABASE Availability;
GO

USE Availability;
GO

-- Room inventory table
CREATE TABLE RoomInventory (
    Id uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
    HotelId uniqueidentifier NOT NULL,
    RoomTypeId uniqueidentifier NOT NULL,
    Date date NOT NULL,
    TotalRooms int NOT NULL,
    AvailableRooms int NOT NULL,
    BlockedRooms int NOT NULL DEFAULT 0,
    MaintenanceRooms int NOT NULL DEFAULT 0,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt datetime2,
    CreatedBy nvarchar(255) NOT NULL,
    UpdatedBy nvarchar(255),
    RowVersion rowversion NOT NULL,
    CONSTRAINT UK_RoomInventory_Date UNIQUE (HotelId, RoomTypeId, Date)
);
GO

-- Pricing rules table
CREATE TABLE PricingRules (
    Id uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
    HotelId uniqueidentifier NOT NULL,
    RoomTypeId uniqueidentifier NOT NULL,
    Name nvarchar(100) NOT NULL,
    Description ntext,
    StartDate date NOT NULL,
    EndDate date NOT NULL,
    DayOfWeek smallint, -- NULL for all days, 0-6 for specific days
    MinLengthOfStay int,
    MaxLengthOfStay int,
    PriceMultiplier decimal(3,2) NOT NULL,
    MinOccupancy int,
    MaxOccupancy int,
    IsActive bit NOT NULL DEFAULT 1,
    Priority int NOT NULL DEFAULT 0,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt datetime2,
    CreatedBy nvarchar(255) NOT NULL,
    UpdatedBy nvarchar(255),
    RowVersion rowversion NOT NULL
);
GO

-- Seasonal rates table
CREATE TABLE SeasonalRates (
    Id uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
    HotelId uniqueidentifier NOT NULL,
    RoomTypeId uniqueidentifier NOT NULL,
    SeasonName nvarchar(100) NOT NULL,
    StartDate date NOT NULL,
    EndDate date NOT NULL,
    BasePriceAdjustment decimal(10,2) NOT NULL,
    IsActive bit NOT NULL DEFAULT 1,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt datetime2,
    CreatedBy nvarchar(255) NOT NULL,
    UpdatedBy nvarchar(255),
    RowVersion rowversion NOT NULL
);
GO

-- Blackout dates table
CREATE TABLE BlackoutDates (
    Id uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
    HotelId uniqueidentifier NOT NULL,
    RoomTypeId uniqueidentifier NULL, -- NULL means applies to all room types
    StartDate date NOT NULL,
    EndDate date NOT NULL,
    Reason ntext,
    IsActive bit NOT NULL DEFAULT 1,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt datetime2,
    CreatedBy nvarchar(255) NOT NULL,
    UpdatedBy nvarchar(255),
    RowVersion rowversion NOT NULL
);
GO

-- Indexes
CREATE INDEX IDX_RoomInventory_Date ON RoomInventory(Date);
CREATE INDEX IDX_RoomInventory_Hotel ON RoomInventory(HotelId);
CREATE INDEX IDX_PricingRules_Date ON PricingRules(StartDate, EndDate);
CREATE INDEX IDX_SeasonalRates_Date ON SeasonalRates(StartDate, EndDate);
CREATE INDEX IDX_BlackoutDates_Date ON BlackoutDates(StartDate, EndDate);
GO

-- Stored Procedures

-- Update room inventory
CREATE PROCEDURE sp_UpdateRoomInventory
    @HotelId uniqueidentifier,
    @RoomTypeId uniqueidentifier,
    @Date date,
    @TotalRooms int,
    @AvailableRooms int,
    @BlockedRooms int,
    @MaintenanceRooms int,
    @UpdatedBy nvarchar(255)
AS
BEGIN
    SET NOCOUNT ON;

    MERGE RoomInventory AS target
    USING (SELECT @HotelId, @RoomTypeId, @Date) AS source (HotelId, RoomTypeId, Date)
    ON target.HotelId = source.HotelId 
        AND target.RoomTypeId = source.RoomTypeId 
        AND target.Date = source.Date
    WHEN MATCHED THEN
        UPDATE SET
            TotalRooms = @TotalRooms,
            AvailableRooms = @AvailableRooms,
            BlockedRooms = @BlockedRooms,
            MaintenanceRooms = @MaintenanceRooms,
            UpdatedAt = GETUTCDATE(),
            UpdatedBy = @UpdatedBy
    WHEN NOT MATCHED THEN
        INSERT (HotelId, RoomTypeId, Date, TotalRooms, AvailableRooms, BlockedRooms, MaintenanceRooms, CreatedBy)
        VALUES (@HotelId, @RoomTypeId, @Date, @TotalRooms, @AvailableRooms, @BlockedRooms, @MaintenanceRooms, @UpdatedBy);
END;
GO

-- Get availability for date range
CREATE PROCEDURE sp_GetAvailability
    @HotelId uniqueidentifier,
    @RoomTypeId uniqueidentifier,
    @StartDate date,
    @EndDate date
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ri.Date,
        ri.TotalRooms,
        ri.AvailableRooms,
        ri.BlockedRooms,
        ri.MaintenanceRooms,
        CASE WHEN bd.Id IS NOT NULL THEN 1 ELSE 0 END as IsBlackout
    FROM RoomInventory ri
    LEFT JOIN BlackoutDates bd ON 
        bd.HotelId = ri.HotelId 
        AND (bd.RoomTypeId IS NULL OR bd.RoomTypeId = ri.RoomTypeId)
        AND ri.Date BETWEEN bd.StartDate AND bd.EndDate
        AND bd.IsActive = 1
    WHERE ri.HotelId = @HotelId
        AND ri.RoomTypeId = @RoomTypeId
        AND ri.Date BETWEEN @StartDate AND @EndDate
    ORDER BY ri.Date;
END;
GO

-- Calculate room price
CREATE PROCEDURE sp_CalculateRoomPrice
    @HotelId uniqueidentifier,
    @RoomTypeId uniqueidentifier,
    @Date date,
    @LengthOfStay int,
    @Occupancy int,
    @FinalPrice decimal(10,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @BasePrice decimal(10,2) = 0;
    DECLARE @Multiplier decimal(3,2) = 1.0;

    -- Get seasonal adjustments
    SELECT @BasePrice = ISNULL(SUM(BasePriceAdjustment), 0)
    FROM SeasonalRates
    WHERE HotelId = @HotelId
        AND RoomTypeId = @RoomTypeId
        AND @Date BETWEEN StartDate AND EndDate
        AND IsActive = 1;

    -- Apply pricing rules
    SELECT @Multiplier = ISNULL(MAX(PriceMultiplier), 1.0)
    FROM PricingRules
    WHERE HotelId = @HotelId
        AND RoomTypeId = @RoomTypeId
        AND @Date BETWEEN StartDate AND EndDate
        AND IsActive = 1
        AND (DayOfWeek IS NULL OR DayOfWeek = DATEPART(WEEKDAY, @Date) - 1)
        AND (MinLengthOfStay IS NULL OR MinLengthOfStay <= @LengthOfStay)
        AND (MaxLengthOfStay IS NULL OR MaxLengthOfStay >= @LengthOfStay)
        AND (MinOccupancy IS NULL OR MinOccupancy <= @Occupancy)
        AND (MaxOccupancy IS NULL OR MaxOccupancy >= @Occupancy);

    -- Calculate final price
    SET @FinalPrice = @BasePrice * @Multiplier;
END;
GO

-- Create pricing rule
CREATE PROCEDURE sp_CreatePricingRule
    @HotelId uniqueidentifier,
    @RoomTypeId uniqueidentifier,
    @Name nvarchar(100),
    @Description ntext,
    @StartDate date,
    @EndDate date,
    @DayOfWeek smallint,
    @MinLengthOfStay int,
    @MaxLengthOfStay int,
    @PriceMultiplier decimal(3,2),
    @MinOccupancy int,
    @MaxOccupancy int,
    @Priority int,
    @CreatedBy nvarchar(255),
    @Id uniqueidentifier OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Id = NEWID();

    INSERT INTO PricingRules (
        Id, HotelId, RoomTypeId, Name, Description, StartDate, EndDate,
        DayOfWeek, MinLengthOfStay, MaxLengthOfStay, PriceMultiplier,
        MinOccupancy, MaxOccupancy, Priority, CreatedBy
    )
    VALUES (
        @Id, @HotelId, @RoomTypeId, @Name, @Description, @StartDate, @EndDate,
        @DayOfWeek, @MinLengthOfStay, @MaxLengthOfStay, @PriceMultiplier,
        @MinOccupancy, @MaxOccupancy, @Priority, @CreatedBy
    );
END;
GO
