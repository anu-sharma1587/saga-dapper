-- Identity Service Database Schema - SQL Server

-- Create database
CREATE DATABASE Identity;
GO

USE Identity;
GO

-- Users table
CREATE TABLE Users (
    Id uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
    Email nvarchar(255) NOT NULL UNIQUE,
    PasswordHash nvarchar(512) NOT NULL,
    FirstName nvarchar(100) NOT NULL,
    LastName nvarchar(100) NOT NULL,
    PhoneNumber nvarchar(20),
    IsActive bit NOT NULL DEFAULT 1,
    TwoFactorEnabled bit NOT NULL DEFAULT 0,
    TwoFactorSecret nvarchar(32),
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt datetime2,
    CreatedBy nvarchar(255) NOT NULL,
    UpdatedBy nvarchar(255),
    RowVersion rowversion NOT NULL
);
GO

-- Roles table
CREATE TABLE Roles (
    Id uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
    Name nvarchar(50) NOT NULL UNIQUE,
    Description nvarchar(255),
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy nvarchar(255) NOT NULL,
    RowVersion rowversion NOT NULL
);
GO

-- User roles mapping
CREATE TABLE UserRoles (
    UserId uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Users(Id),
    RoleId uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Roles(Id),
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy nvarchar(255) NOT NULL,
    PRIMARY KEY (UserId, RoleId)
);
GO

-- Refresh tokens
CREATE TABLE RefreshTokens (
    Id uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
    UserId uniqueidentifier NOT NULL FOREIGN KEY REFERENCES Users(Id),
    Token nvarchar(512) NOT NULL UNIQUE,
    ExpiresAt datetime2 NOT NULL,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy nvarchar(255) NOT NULL,
    IsRevoked bit NOT NULL DEFAULT 0,
    RevokedAt datetime2,
    RevokedBy nvarchar(255)
);
GO

-- Stored Procedures

-- Create user
CREATE PROCEDURE sp_CreateUser
    @Email nvarchar(255),
    @PasswordHash nvarchar(512),
    @FirstName nvarchar(100),
    @LastName nvarchar(100),
    @PhoneNumber nvarchar(20),
    @CreatedBy nvarchar(255),
    @Id uniqueidentifier OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Id = NEWID();
    
    INSERT INTO Users (Id, Email, PasswordHash, FirstName, LastName, PhoneNumber, CreatedBy)
    VALUES (@Id, @Email, @PasswordHash, @FirstName, @LastName, @PhoneNumber, @CreatedBy);
END;
GO

-- Get user by email
CREATE PROCEDURE sp_GetUserByEmail
    @Email nvarchar(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id, Email, PasswordHash, FirstName, LastName, 
           PhoneNumber, IsActive, TwoFactorEnabled,
           CreatedAt, UpdatedAt, RowVersion
    FROM Users
    WHERE Email = @Email;
END;
GO

-- Get user roles
CREATE PROCEDURE sp_GetUserRoles
    @UserId uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT r.Id AS RoleId, r.Name AS RoleName
    FROM Roles r
    JOIN UserRoles ur ON ur.RoleId = r.Id
    WHERE ur.UserId = @UserId;
END;
GO

-- Create refresh token
CREATE PROCEDURE sp_CreateRefreshToken
    @UserId uniqueidentifier,
    @Token nvarchar(512),
    @ExpiresAt datetime2,
    @CreatedBy nvarchar(255),
    @Id uniqueidentifier OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Id = NEWID();
    
    INSERT INTO RefreshTokens (Id, UserId, Token, ExpiresAt, CreatedBy)
    VALUES (@Id, @UserId, @Token, @ExpiresAt, @CreatedBy);
END;
GO

-- Validate refresh token
CREATE PROCEDURE sp_ValidateRefreshToken
    @Token nvarchar(512)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id, UserId, ExpiresAt, IsRevoked
    FROM RefreshTokens
    WHERE Token = @Token;
END;
GO

-- Revoke refresh token
CREATE PROCEDURE sp_RevokeRefreshToken
    @Token nvarchar(512),
    @RevokedBy nvarchar(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE RefreshTokens
    SET IsRevoked = 1,
        RevokedAt = GETUTCDATE(),
        RevokedBy = @RevokedBy
    WHERE Token = @Token;
END;
GO

-- Seed Data
INSERT INTO Roles (Id, Name, Description, CreatedBy) VALUES
    (NEWID(), 'Admin', 'System administrator', 'system'),
    (NEWID(), 'Manager', 'Hotel manager', 'system'),
    (NEWID(), 'Staff', 'Hotel staff', 'system'),
    (NEWID(), 'Guest', 'Hotel guest', 'system');
GO
