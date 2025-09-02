-- Hotel Inventory Service Database Schema - SQL Server

-- Create database
CREATE DATABASE HotelInventory;
GO

USE HotelInventory;
GO

-- Hotels table
CREATE TABLE Hotels (
    Id uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
    Name nvarchar(255) NOT NULL,
    Description ntext,
    AddressLine1 nvarchar(255) NOT NULL,
    AddressLine2 nvarchar(255),
    City nvarchar(100) NOT NULL,
    State nvarchar(100),
    Country nvarchar(100) NOT NULL,
    PostalCode nvarchar(20) NOT NULL,
    PhoneNumber nvarchar(20) NOT NULL,
    Email nvarchar(255) NOT NULL,
    Timezone nvarchar(50) NOT NULL,
    CheckInTime time NOT NULL,
    CheckOutTime time NOT NULL,
    Rating smallint,
    IsActive bit NOT NULL DEFAULT 1,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt datetime2,
    CreatedBy nvarchar(255) NOT NULL,
    UpdatedBy nvarchar(255),
    RowVersion rowversion NOT NULL
);
GO

-- Hotel amenities table
CREATE TABLE HotelAmenities (
    Id uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
    HotelId uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Hotels(Id),
    Name nvarchar(100) NOT NULL,
    Description ntext,
    IconUrl nvarchar(255),
    IsActive bit NOT NULL DEFAULT 1,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy nvarchar(255) NOT NULL,
    RowVersion rowversion NOT NULL
);
GO

-- Room types table
CREATE TABLE RoomTypes (
    Id uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
    HotelId uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Hotels(Id),
    Name nvarchar(100) NOT NULL,
    Description ntext,
    Capacity int NOT NULL,
    BedType nvarchar(50) NOT NULL,
    RoomSize int NOT NULL,
    BasePrice decimal(10,2) NOT NULL,
    IsActive bit NOT NULL DEFAULT 1,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt datetime2,
    CreatedBy nvarchar(255) NOT NULL,
    UpdatedBy nvarchar(255),
    RowVersion rowversion NOT NULL
);
GO

-- Room type amenities table
CREATE TABLE RoomTypeAmenities (
    Id uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
    RoomTypeId uniqueidentifier NOT NULL FOREIGN KEY REFERENCES RoomTypes(Id),
    Name nvarchar(100) NOT NULL,
    Description ntext,
    IconUrl nvarchar(255),
    IsActive bit NOT NULL DEFAULT 1,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy nvarchar(255) NOT NULL,
    RowVersion rowversion NOT NULL
);
GO

-- Room type images table
CREATE TABLE RoomTypeImages (
    Id uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
    RoomTypeId uniqueidentifier NOT NULL FOREIGN KEY REFERENCES RoomTypes(Id),
    ImageUrl nvarchar(255) NOT NULL,
    Caption nvarchar(255),
    IsPrimary bit NOT NULL DEFAULT 0,
    DisplayOrder int NOT NULL DEFAULT 0,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy nvarchar(255) NOT NULL,
    RowVersion rowversion NOT NULL
);
GO

-- Rate plans table
CREATE TABLE RatePlans (
    Id uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
    HotelId uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Hotels(Id),
    RoomTypeId uniqueidentifier NOT NULL FOREIGN KEY REFERENCES RoomTypes(Id),
    Name nvarchar(100) NOT NULL,
    Description ntext,
    CancellationPolicy ntext,
    PriceMultiplier decimal(3,2) NOT NULL DEFAULT 1.00,
    IsRefundable bit NOT NULL DEFAULT 1,
    IsActive bit NOT NULL DEFAULT 1,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt datetime2,
    CreatedBy nvarchar(255) NOT NULL,
    UpdatedBy nvarchar(255),
    RowVersion rowversion NOT NULL
);
GO

-- Stored Procedures

-- Create hotel
CREATE PROCEDURE sp_CreateHotel
    @Name nvarchar(255),
    @Description ntext,
    @AddressLine1 nvarchar(255),
    @AddressLine2 nvarchar(255),
    @City nvarchar(100),
    @State nvarchar(100),
    @Country nvarchar(100),
    @PostalCode nvarchar(20),
    @PhoneNumber nvarchar(20),
    @Email nvarchar(255),
    @Timezone nvarchar(50),
    @CheckInTime time,
    @CheckOutTime time,
    @CreatedBy nvarchar(255),
    @Id uniqueidentifier OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Id = NEWID();
    
    INSERT INTO Hotels (
        Id, Name, Description, AddressLine1, AddressLine2, City, State, Country,
        PostalCode, PhoneNumber, Email, Timezone, CheckInTime, CheckOutTime, CreatedBy
    )
    VALUES (
        @Id, @Name, @Description, @AddressLine1, @AddressLine2, @City, @State, @Country,
        @PostalCode, @PhoneNumber, @Email, @Timezone, @CheckInTime, @CheckOutTime, @CreatedBy
    );
END;
GO

-- Get hotel by id
CREATE PROCEDURE sp_GetHotelById
    @Id uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id, Name, Description, AddressLine1, AddressLine2,
           City, State, Country, PostalCode, PhoneNumber,
           Email, Timezone, CheckInTime, CheckOutTime,
           Rating, IsActive, CreatedAt, UpdatedAt, RowVersion
    FROM Hotels
    WHERE Id = @Id;
END;
GO

-- Create room type
CREATE PROCEDURE sp_CreateRoomType
    @HotelId uniqueidentifier,
    @Name nvarchar(100),
    @Description ntext,
    @Capacity int,
    @BedType nvarchar(50),
    @RoomSize int,
    @BasePrice decimal(10,2),
    @CreatedBy nvarchar(255),
    @Id uniqueidentifier OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Id = NEWID();
    
    INSERT INTO RoomTypes (
        Id, HotelId, Name, Description, Capacity, BedType,
        RoomSize, BasePrice, CreatedBy
    )
    VALUES (
        @Id, @HotelId, @Name, @Description, @Capacity, @BedType,
        @RoomSize, @BasePrice, @CreatedBy
    );
END;
GO

-- Get room types by hotel
CREATE PROCEDURE sp_GetRoomTypesByHotel
    @HotelId uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id, Name, Description, Capacity, BedType,
           RoomSize, BasePrice, IsActive, CreatedAt,
           UpdatedAt, RowVersion
    FROM RoomTypes
    WHERE HotelId = @HotelId;
END;
GO
