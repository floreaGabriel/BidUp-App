CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),       -- Auto-incrementing primary key
    FullName NVARCHAR(100) NOT NULL,            -- User's full name
    Email NVARCHAR(255) NOT NULL UNIQUE,        -- User's email, unique
    PasswordHash NVARCHAR(255) NOT NULL,        -- Password hash for secure storage
     Role NVARCHAR(20) CHECK (Role IN ('Bidder', 'Admin', 'Seller')),                 -- Role of the user (e.g., Bidder, Admin, Seller)
    BirthDate DATE NOT NULL,                    -- User's date of birth
    ProfilePicturePath NVARCHAR(500) NULL,      -- Path to profile picture, nullable in case not provided
    CreatedAt DATETIME DEFAULT GETDATE()        -- Date and time the user was created, with default value of current date
);

DROP TABLE IF EXISTS Users;
