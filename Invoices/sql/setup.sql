-- CREATE DB 
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'InvoiceDB')
BEGIN
    CREATE DATABASE InvoiceDB;
END
GO

USE InvoiceDB;
GO

-- ///////////////////////////////
-- CREATE TABLE

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Invoices' AND xtype = 'U')
BEGIN
    CREATE TABLE Invoices (
        Id           INT            PRIMARY KEY IDENTITY(1,1),
        UniqueCode   NVARCHAR(30)   UNIQUE NOT NULL,
        CustomerName NVARCHAR(255)  NOT NULL,
        Amount       DECIMAL(18, 2) NOT NULL,
        [Status]     NVARCHAR(50)   NOT NULL
                         CONSTRAINT CK_Invoices_Status
                         CHECK ([Status] IN ('Draft', 'Issued', 'Sent', 'Paid', 'Cancelled')),
        CreatedAt    DATETIME       NOT NULL CONSTRAINT DF_Invoices_CreatedAt DEFAULT GETDATE(),
        UpdatedAt    DATETIME       NULL,
        IsDeleted    BIT            NOT NULL CONSTRAINT DF_Invoices_IsDeleted DEFAULT 0
    );
END
GO


-- ///////////////////////////////
-- CREATE STORED PROCEDURES


IF OBJECT_ID('sp_invoice_create', 'P') IS NOT NULL DROP PROCEDURE sp_invoice_create;
GO

CREATE PROCEDURE sp_invoice_create
    @CustomerName NVARCHAR(255),
    @Amount       DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    -- Generate collision-resistant unique code: INV-20260507-A3F1B92C
   DECLARE @UniqueCode NVARCHAR(30) = 'INV-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-' + CAST(ABS(CHECKSUM(NEWID())) % 10000 AS NVARCHAR(4));

    INSERT INTO Invoices (UniqueCode, CustomerName, Amount, [Status], CreatedAt, IsDeleted)
    VALUES (@UniqueCode, @CustomerName, @Amount, 'Draft', GETDATE(), 0);

    DECLARE @NewId INT = SCOPE_IDENTITY();

    -- Return the full created row so caller gets UniqueCode + CreatedAt immediately
    SELECT
        Id, UniqueCode, CustomerName, Amount, [Status],
        CreatedAt, UpdatedAt, IsDeleted
    FROM Invoices
    WHERE Id = @NewId;
END
GO



IF OBJECT_ID('sp_invoice_get_by_id', 'P') IS NOT NULL DROP PROCEDURE sp_invoice_get_by_id;
GO

CREATE PROCEDURE sp_invoice_get_by_id
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Invoices WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        RAISERROR('Invoice with Id %d not found or has been deleted.', 16, 1, @Id);
        RETURN;
    END

    SELECT
        Id, UniqueCode, CustomerName, Amount, [Status],
        CreatedAt, UpdatedAt, IsDeleted
    FROM Invoices
    WHERE Id = @Id AND IsDeleted = 0;
END
GO


IF OBJECT_ID('sp_invoice_get_all', 'P') IS NOT NULL DROP PROCEDURE sp_invoice_get_all;
GO

CREATE PROCEDURE sp_invoice_get_all
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id, UniqueCode, CustomerName, Amount, [Status],
        CreatedAt, UpdatedAt, IsDeleted
    FROM Invoices
    WHERE IsDeleted = 0
    ORDER BY CreatedAt DESC;
END
GO


IF OBJECT_ID('sp_invoice_update', 'P') IS NOT NULL DROP PROCEDURE sp_invoice_update;
GO

CREATE PROCEDURE sp_invoice_update
    @Id           INT,
    @CustomerName NVARCHAR(255),
    @Amount       DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Invoices WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        RAISERROR('Invoice with Id %d not found or has been deleted.', 16, 1, @Id);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Invoices WHERE Id = @Id AND [Status] IN ('Paid', 'Cancelled'))
    BEGIN
        RAISERROR('Cannot update an invoice that is already Paid or Cancelled.', 16, 1);
        RETURN;
    END

    UPDATE Invoices
    SET
        CustomerName = @CustomerName,
        Amount       = @Amount,
        UpdatedAt    = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    SELECT
        Id, UniqueCode, CustomerName, Amount, [Status],
        CreatedAt, UpdatedAt, IsDeleted
    FROM Invoices
    WHERE Id = @Id;
END
GO



IF OBJECT_ID('sp_invoice_update_status', 'P') IS NOT NULL DROP PROCEDURE sp_invoice_update_status;
GO

CREATE PROCEDURE sp_invoice_update_status
    @InvoiceId INT,
    @NewStatus NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentStatus NVARCHAR(50);

    SELECT @CurrentStatus = [Status]
    FROM Invoices
    WHERE Id = @InvoiceId AND IsDeleted = 0;

    -- Invoice not found or is deleted
    IF @CurrentStatus IS NULL
    BEGIN
        RAISERROR('Invoice with Id %d not found or has been deleted.', 16, 1, @InvoiceId);
        RETURN;
    END

    IF @NewStatus NOT IN ('Draft', 'Issued', 'Sent', 'Paid', 'Cancelled')
    BEGIN
        RAISERROR('Unknown status value: %s.', 16, 1, @NewStatus);
        RETURN;
    END

    IF @CurrentStatus = @NewStatus
    BEGIN
        RAISERROR('Invoice is already in status %s.', 16, 1, @NewStatus);
        RETURN;
    END

    DECLARE @IsValid BIT = 0;

    IF @CurrentStatus = 'Draft'  AND @NewStatus = 'Issued'    SET @IsValid = 1;
    IF @CurrentStatus = 'Issued' AND @NewStatus = 'Sent'      SET @IsValid = 1;
    IF @CurrentStatus = 'Sent'   AND @NewStatus = 'Paid'      SET @IsValid = 1;
    IF @CurrentStatus IN ('Draft', 'Issued', 'Sent') AND @NewStatus = 'Cancelled' SET @IsValid = 1;

    IF @IsValid = 0
    BEGIN
        RAISERROR('Invalid status transition: %s → %s is not allowed.', 16, 1, @CurrentStatus, @NewStatus);
        RETURN;
    END

    UPDATE Invoices
    SET [Status]  = @NewStatus,
        UpdatedAt = GETDATE()
    WHERE Id = @InvoiceId;

    SELECT
        Id, UniqueCode, CustomerName, Amount, [Status],
        CreatedAt, UpdatedAt, IsDeleted
    FROM Invoices
    WHERE Id = @InvoiceId;
END
GO


IF OBJECT_ID('sp_invoice_soft_delete', 'P') IS NOT NULL DROP PROCEDURE sp_invoice_soft_delete;
GO

CREATE PROCEDURE sp_invoice_soft_delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Invoices WHERE Id = @Id)
    BEGIN
        RAISERROR('Invoice with Id %d not found.', 16, 1, @Id);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Invoices WHERE Id = @Id AND IsDeleted = 1)
    BEGIN
        RAISERROR('Invoice with Id %d is already deleted.', 16, 1, @Id);
        RETURN;
    END

    UPDATE Invoices
    SET IsDeleted = 1,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END
GO

-- ///////////////////////////////
-- SEED DB
INSERT INTO Invoices (UniqueCode, CustomerName, Amount, [Status], CreatedAt, IsDeleted)
VALUES
    ('INV-20260507-SEED0001', 'Alice Johnson',  1500.00, 'Draft',     GETDATE(), 0),
    ('INV-20260507-SEED0002', 'Bob Smith',      3200.50, 'Issued',    GETDATE(), 0),
    ('INV-20260507-SEED0003', 'Carol Williams', 750.00,  'Sent',      GETDATE(), 0),
    ('INV-20260507-SEED0004', 'David Brown',    9999.99, 'Paid',      GETDATE(), 0),
    ('INV-20260507-SEED0005', 'Eve Davis',      450.25,  'Cancelled', GETDATE(), 0);
GO