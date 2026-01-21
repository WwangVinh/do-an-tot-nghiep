/* =============================================================
   DATABASE SCRIPT: WEBSITE BÁN Ô TÔ TÍCH HỢP AI
   CREATED FOR: ĐỒ ÁN TỐT NGHIỆP
   PLATFORM: SQL SERVER
   ============================================================= */

-- 1. TẠO DATABASE (Nếu đã có thì xóa đi tạo lại để làm sạch)
USE master;
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'CarSalesDB')
BEGIN
    ALTER DATABASE CarSalesDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CarSalesDB;
END
GO

CREATE DATABASE CarSalesDB;
GO

USE CarSalesDB;
GO

-- =============================================================
-- 2. TẠO BẢNG (TABLE SCHEMA)
-- =============================================================

-- Bảng Hãng xe (Brands)
CREATE TABLE Brands (
    BrandId INT IDENTITY(1,1) PRIMARY KEY,
    BrandName NVARCHAR(100) NOT NULL,
    CountryOrigin NVARCHAR(100), -- Xuất xứ: Nhật, Đức, Việt Nam...
    LogoUrl NVARCHAR(500),
    Description NTEXT
);
GO

-- Bảng Dòng xe (Models) - VD: Camry, VF8, C-Class
CREATE TABLE CarModels (
    ModelId INT IDENTITY(1,1) PRIMARY KEY,
    BrandId INT NOT NULL,
    ModelName NVARCHAR(100) NOT NULL,
    BodyType NVARCHAR(50), -- Sedan, SUV, Crossover, Hatchback
    CONSTRAINT FK_CarModels_Brands FOREIGN KEY (BrandId) REFERENCES Brands(BrandId) ON DELETE CASCADE
);
GO

-- Bảng Sản phẩm Xe (Cars) - Chi tiết từng chiếc xe bán ra
CREATE TABLE Cars (
    CarId INT IDENTITY(1,1) PRIMARY KEY,
    ModelId INT NOT NULL,
    VersionName NVARCHAR(100), -- VD: 2.5Q, Eco, Plus, AMG Line
    YearOfManufacture INT,
    Price DECIMAL(18, 0), -- Giá tiền (VND)
    Color NVARCHAR(50),
    FuelType NVARCHAR(50), -- Xăng, Dầu, Điện, Hybrid
    Transmission NVARCHAR(50), -- Số sàn, Số tự động
    StockQuantity INT DEFAULT 1,
    IsNew BIT DEFAULT 1, -- 1: Xe mới, 0: Xe cũ
    ThumbImage NVARCHAR(500), -- Ảnh đại diện hiển thị ở danh sách
    Description NTEXT,
    Status BIT DEFAULT 1, -- 1: Đang bán, 0: Ẩn
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Cars_CarModels FOREIGN KEY (ModelId) REFERENCES CarModels(ModelId) ON DELETE CASCADE
);
GO

-- Bảng Danh mục Thông số (Specifications)
CREATE TABLE Specifications (
    SpecId INT IDENTITY(1,1) PRIMARY KEY,
    SpecName NVARCHAR(100) NOT NULL, -- Tên thông số (Dài x Rộng x Cao, Công suất...)
    Unit NVARCHAR(50), -- Đơn vị (mm, hp, Nm, Lít)
    Category NVARCHAR(50) -- Nhóm (Kích thước, Động cơ, Tiện nghi)
);
GO

-- Bảng Giá trị Thông số của từng xe (CarSpecifications)
CREATE TABLE CarSpecifications (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CarId INT NOT NULL,
    SpecId INT NOT NULL,
    Value NVARCHAR(255) NOT NULL, -- Giá trị thực tế
    CONSTRAINT FK_CarSpecs_Cars FOREIGN KEY (CarId) REFERENCES Cars(CarId) ON DELETE CASCADE,
    CONSTRAINT FK_CarSpecs_Specs FOREIGN KEY (SpecId) REFERENCES Specifications(SpecId) ON DELETE CASCADE
);
GO

-- Bảng Hình ảnh xe (CarImages) - Thư viện ảnh
CREATE TABLE CarImages (
    ImageId INT IDENTITY(1,1) PRIMARY KEY,
    CarId INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    IsThumbnail BIT DEFAULT 0,
    CONSTRAINT FK_CarImages_Cars FOREIGN KEY (CarId) REFERENCES Cars(CarId) ON DELETE CASCADE
);
GO

-- Bảng Người dùng (Users)
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20),
    Address NVARCHAR(255),
    Role NVARCHAR(20) DEFAULT 'Customer', -- Customer, Admin, Staff
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- Bảng Đặt lịch lái thử (TestDriveBookings)
CREATE TABLE TestDriveBookings (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT, -- Có thể Null nếu khách vãng lai
    CarId INT NOT NULL,
    CustomerName NVARCHAR(100), -- Lưu lại tên nếu khách vãng lai
    CustomerPhone NVARCHAR(20),
    BookingDate DATETIME NOT NULL,
    Note NTEXT,
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Confirmed, Cancelled, Completed
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Bookings_Cars FOREIGN KEY (CarId) REFERENCES Cars(CarId),
    CONSTRAINT FK_Bookings_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

-- Bảng Phiên chat AI (ChatSessions)
CREATE TABLE ChatSessions (
    SessionId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    UserId INT, -- Null nếu khách chưa đăng nhập
    Title NVARCHAR(200), -- Tự động đặt tên title dựa trên nội dung chat đầu tiên
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_ChatSessions_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL
);
GO

-- Bảng Tin nhắn Chat (ChatMessages)
CREATE TABLE ChatMessages (
    MessageId BIGINT IDENTITY(1,1) PRIMARY KEY,
    SessionId UNIQUEIDENTIFIER NOT NULL,
    IsUserMessage BIT NOT NULL, -- 1: User hỏi, 0: AI trả lời
    Content NTEXT NOT NULL,
    Timestamp DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_ChatMessages_Sessions FOREIGN KEY (SessionId) REFERENCES ChatSessions(SessionId) ON DELETE CASCADE
);
GO

-- =============================================================
-- 3. SEED DATA (DỮ LIỆU MẪU ĐỂ TEST)
-- =============================================================

-- Thêm Hãng xe
INSERT INTO Brands (BrandName, CountryOrigin, LogoUrl) VALUES 
(N'VinFast', N'Việt Nam', 'vinfast-logo.png'),
(N'Toyota', N'Nhật Bản', 'toyota-logo.png'),
(N'Mercedes-Benz', N'Đức', 'mercedes-logo.png');

-- Thêm Dòng xe (Model)
INSERT INTO CarModels (BrandId, ModelName, BodyType) VALUES 
(1, N'VF 8', N'SUV'), -- VinFast
(2, N'Camry', N'Sedan'), -- Toyota
(3, N'C-Class', N'Sedan'); -- Mercedes

-- Thêm Xe cụ thể (Cars)
INSERT INTO Cars (ModelId, VersionName, YearOfManufacture, Price, Color, FuelType, Transmission, Description) VALUES 
(1, N'Eco', 2024, 1090000000, N'Xanh Dương', N'Điện', N'Tự động', N'Mẫu SUV điện thông minh cỡ D'),
(2, N'2.5Q', 2024, 1405000000, N'Đen', N'Xăng', N'Tự động', N'Doanh nhân, sang trọng, bền bỉ'),
(3, N'C300 AMG', 2024, 2199000000, N'Trắng', N'Xăng', N'Tự động', N'Thể thao, đẳng cấp Đức');

-- Thêm Danh mục thông số (Specifications)
INSERT INTO Specifications (SpecName, Unit, Category) VALUES 
(N'Dài x Rộng x Cao', N'mm', N'Kích thước'),
(N'Công suất tối đa', N'Hp', N'Động cơ'),
(N'Momen xoắn cực đại', N'Nm', N'Động cơ'),
(N'Số chỗ ngồi', N'Chỗ', N'Nội thất');

-- Gán thông số cho xe VinFast VF 8 (CarId = 1)
INSERT INTO CarSpecifications (CarId, SpecId, Value) VALUES 
(1, 1, N'4750 x 1934 x 1667'), -- Kích thước
(1, 2, N'349'), -- Công suất
(1, 3, N'500'), -- Momen xoắn
(1, 4, N'5'); -- Số chỗ

-- Gán thông số cho xe Toyota Camry (CarId = 2)
INSERT INTO CarSpecifications (CarId, SpecId, Value) VALUES 
(2, 1, N'4885 x 1840 x 1445'), 
(2, 2, N'207'), 
(2, 3, N'250'), 
(2, 4, N'5');

-- Xong!
PRINT 'Database Setup Completed Successfully!';