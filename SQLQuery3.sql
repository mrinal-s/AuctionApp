-- Create Users table
CREATE DATABASE AuctionBidData;
GO
select * from Users
USE AuctionBidData;
GO
update users set LastName='test'
ALTER TABLE users
add 
FirstName NVARCHAR(255),
LastName NVARCHAR(255),
IsRecieveOutbidEmails BIT DEFAULT 0
ALTER TABLE Users
ADD 
    AccessFailedCount INT DEFAULT 0, 
    ConcurrencyStamp NVARCHAR(255),
    EmailConfirmed BIT DEFAULT 0,
    LockoutEnabled BIT DEFAULT 0,
    LockoutEnd DATETIME NULL,
    NormalizedEmail NVARCHAR(255),
    NormalizedUserName NVARCHAR(255),
    PhoneNumber NVARCHAR(50) NULL,
    PhoneNumberConfirmed BIT DEFAULT 0,
    SecurityStamp NVARCHAR(255),
    TwoFactorEnabled BIT DEFAULT 0;

-- Create Users table
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY, -- Remove auto generation
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

select *  from Users
-- Drop AuctionItems table if it exists
IF OBJECT_ID('AuctionItems', 'U') IS NOT NULL
    DROP TABLE AuctionItems;

-- Create AuctionItems table
CREATE TABLE AuctionItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY, -- Remove auto generation
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    StartingBid DECIMAL(18, 2) NOT NULL,
    EndDate DATETIME NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE NO ACTION -- No cascade delete
);

-- Drop Bids table if it exists
IF OBJECT_ID('Bids', 'U') IS NOT NULL
    DROP TABLE Bids;

-- Create Bids table
CREATE TABLE Bids (
    Id UNIQUEIDENTIFIER PRIMARY KEY, -- Remove auto generation
    Amount DECIMAL(18, 2) NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    AuctionItemId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE NO ACTION, -- No cascade delete
    FOREIGN KEY (AuctionItemId) REFERENCES AuctionItems(Id) ON DELETE NO ACTION -- No cascade delete
);

-- Add indexes for faster lookup
CREATE INDEX IDX_UserEmail ON Users(Email);
CREATE INDEX IDX_AuctionItemUser ON AuctionItems(UserId);
CREATE INDEX IDX_BidsAuctionItem ON Bids(AuctionItemId);
CREATE INDEX IDX_BidsUser ON Bids(UserId);

-- Ensure bid amount can't be less than 0
ALTER TABLE Bids
ADD CONSTRAINT CHK_BidAmount CHECK (Amount >= 0);
