-- Create database
CREATE DATABASE Project1DB;
GO

-- Use the database
USE Project1DB;
GO

-- Create users table
CREATE TABLE users (
    user_id INT PRIMARY KEY,
    user_birth DATE,
    user_address NVARCHAR(255),
    user_identity INT,
    user_phone INT,
    user_name NVARCHAR(255)
);
GO

-- Create passbook table
CREATE TABLE passbook (
    user_id INT,
    passbook_id INT PRIMARY KEY,
    balance FLOAT,
    startDate DATE,
    endDate DATE,
    term VARCHAR(50),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);
GO

-- Create interestRates table
CREATE TABLE interestRates (
    term VARCHAR(50) PRIMARY KEY,
    interestRate FLOAT
);
GO

-- Create Transactions table
CREATE TABLE Transactions (
    transactionID INT PRIMARY KEY,
    user_id INT,
    transactionDate DATE,
    transactionAmount FLOAT,
    transactionType VARCHAR(50),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);
GO

-- Insert sample data into users table
INSERT INTO users (user_id, user_birth, user_address, user_identity, user_phone, user_name)
VALUES
(1, '1990-01-01', '123 Main St', 123456789, 5551234, 'John Doe'),
(2, '1985-05-15', '456 Elm St', 987654321, 5555678, 'Jane Smith');
GO

-- Insert sample data into interestRates table
INSERT INTO interestRates (term, interestRate)
VALUES
('Không kỳ hạn', 0.0015),
('3 tháng', 0.005),
('6 tháng', 0.0055);
GO

-- Insert sample data into passbook table
INSERT INTO passbook (user_id, passbook_id, balance, startDate, endDate, term)
VALUES
(1, 1001, 1500000, '2023-01-01', '2023-04-01', '3 tháng'),
(2, 1002, 2000000, '2023-01-01', '2023-07-01', '6 tháng');
GO

-- Insert sample data into Transactions table
INSERT INTO Transactions (transactionID, user_id, transactionDate, transactionAmount, transactionType)
VALUES
(1, 1, '2023-01-01', 1500000, 'Deposit'),
(2, 2, '2023-01-01', 2000000, 'Deposit'),
(3, 1, '2023-04-01', 100000, 'Withdraw'),
(4, 2, '2023-07-01', 1500000, 'Withdraw');
GO




CREATE PROCEDURE DepositMoney
    @user_id INT,
    @passbook_id INT,
    @amount FLOAT
AS
BEGIN
    DECLARE @term VARCHAR(50);
    DECLARE @current_balance FLOAT;
    DECLARE @startDate DATE;
    DECLARE @interestRate FLOAT;
    
    SELECT @term = term, @current_balance = balance, @startDate = startDate
    FROM passbook
    WHERE passbook_id = @passbook_id;

    IF @term = 'Không kỳ hạn' OR 
       (@term = '3 tháng' AND DATEADD(MONTH, 3, @startDate) = GETDATE()) OR
       (@term = '6 tháng' AND DATEADD(MONTH, 6, @startDate) = GETDATE())
    BEGIN
        IF @amount >= 100000
        BEGIN
            UPDATE passbook
            SET balance = @current_balance + @amount
            WHERE passbook_id = @passbook_id;
            
            INSERT INTO Transactions (user_id, transactionDate, transactionAmount, transactionType)
            VALUES (@user_id, GETDATE(), @amount, 'Deposit');
        END
        ELSE
        BEGIN
            PRINT 'Amount to deposit must be at least 100,000';
        END
    END
    ELSE
    BEGIN
        PRINT 'Cannot deposit at this time';
    END
END;
GO






CREATE TRIGGER ClosePassbook
ON Transactions
AFTER INSERT
AS
BEGIN
    DECLARE @transactionType VARCHAR(50);
    DECLARE @transactionAmount FLOAT;
    DECLARE @passbook_id INT;
    
    SELECT @transactionType = transactionType, @transactionAmount = transactionAmount, @passbook_id = passbook_id
    FROM inserted
    INNER JOIN passbook ON inserted.user_id = passbook.user_id;

    IF @transactionType = 'Withdraw'
    BEGIN
        UPDATE passbook
        SET balance = 0
        WHERE passbook_id = @passbook_id AND @transactionAmount = balance;
    END
END;
GO
