-- Create Users table
CREATE DATABASE AuctionBidDB;
GO

USE AuctionBidDB;
GO

-- Create Users table
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER   PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
drop table AuctionItems
-- Create AuctionItems table
CREATE TABLE AuctionItems (
    Id UNIQUEIDENTIFIER  PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    StartingBid DECIMAL(18, 2) NOT NULL,
    EndDate DATETIME NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE NO ACTION -- No cascade delete
);

-- Create Bids table
CREATE TABLE Bids (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
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
