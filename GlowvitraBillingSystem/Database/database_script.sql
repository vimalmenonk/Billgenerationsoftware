-- Glowvitra Billing Database Script
CREATE DATABASE GlowvitraBillingDB;
GO

USE GlowvitraBillingDB;
GO

CREATE TABLE State (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE SellerProfile (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SellerName NVARCHAR(150) NOT NULL,
    Address NVARCHAR(250) NOT NULL,
    Gstin NVARCHAR(20) NOT NULL,
    StateId INT NOT NULL,
    CONSTRAINT FK_SellerProfile_State FOREIGN KEY (StateId) REFERENCES State(Id)
);

CREATE TABLE Product (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    HsnCode NVARCHAR(20) NOT NULL,
    GstPercent DECIMAL(5,2) NOT NULL
);

CREATE TABLE Invoice (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceNumber NVARCHAR(20) NOT NULL UNIQUE,
    InvoiceDate DATETIME2 NOT NULL,
    CustomerName NVARCHAR(150) NOT NULL,
    CustomerPhone NVARCHAR(20) NOT NULL,
    CustomerAddress NVARCHAR(250) NOT NULL,
    CustomerStateId INT NOT NULL,
    Pincode NVARCHAR(10) NOT NULL,
    Subtotal DECIMAL(18,2) NOT NULL,
    CgstAmount DECIMAL(18,2) NOT NULL,
    SgstAmount DECIMAL(18,2) NOT NULL,
    IgstAmount DECIMAL(18,2) NOT NULL,
    RoundOff DECIMAL(18,2) NOT NULL,
    GrandTotal DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_Invoice_State FOREIGN KEY (CustomerStateId) REFERENCES State(Id)
);

CREATE TABLE InvoiceItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductName NVARCHAR(150) NOT NULL,
    HsnCode NVARCHAR(20) NOT NULL,
    Quantity INT NOT NULL,
    BasePrice DECIMAL(18,2) NOT NULL,
    TaxAmount DECIMAL(18,2) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_InvoiceItems_Invoice FOREIGN KEY (InvoiceId) REFERENCES Invoice(Id),
    CONSTRAINT FK_InvoiceItems_Product FOREIGN KEY (ProductId) REFERENCES Product(Id)
);

INSERT INTO State (Name) VALUES
('Andhra Pradesh'),('Arunachal Pradesh'),('Assam'),('Bihar'),('Chhattisgarh'),('Goa'),('Gujarat'),
('Haryana'),('Himachal Pradesh'),('Jharkhand'),('Karnataka'),('Kerala'),('Madhya Pradesh'),
('Maharashtra'),('Manipur'),('Meghalaya'),('Mizoram'),('Nagaland'),('Odisha'),('Punjab'),
('Rajasthan'),('Sikkim'),('Tamil Nadu'),('Telangana'),('Tripura'),('Uttar Pradesh'),
('Uttarakhand'),('West Bengal'),('Andaman and Nicobar Islands'),('Chandigarh'),
('Dadra and Nagar Haveli and Daman and Diu'),('Delhi'),('Jammu and Kashmir'),('Ladakh'),
('Lakshadweep'),('Puducherry');

INSERT INTO SellerProfile (SellerName, Address, Gstin, StateId)
VALUES ('Glowvitra', 'Thrissur, Kerala', '32ABCDE1234F1Z5', (SELECT Id FROM State WHERE Name = 'Kerala'));

INSERT INTO Product (Name, HsnCode, GstPercent) VALUES
('Astronaut Galaxy Projector', '9031', 18.00),
('Floor Corner RGB Lamp', '9405', 18.00),
('Smart LED Strip Light', '8539', 18.00),
('Wireless Charging Desk Lamp', '9405', 18.00),
('Portable Mini Projector', '8528', 18.00);
