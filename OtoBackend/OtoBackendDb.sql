USE [master]
GO
/****** Object:  Database [oto]    Script Date: 3/5/2026 0:28:27 ******/
CREATE DATABASE [oto]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'oto', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER16\MSSQL\DATA\oto.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'oto_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER16\MSSQL\DATA\oto_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [oto] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [oto].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [oto] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [oto] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [oto] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [oto] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [oto] SET ARITHABORT OFF 
GO
ALTER DATABASE [oto] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [oto] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [oto] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [oto] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [oto] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [oto] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [oto] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [oto] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [oto] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [oto] SET  ENABLE_BROKER 
GO
ALTER DATABASE [oto] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [oto] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [oto] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [oto] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [oto] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [oto] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [oto] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [oto] SET RECOVERY FULL 
GO
ALTER DATABASE [oto] SET  MULTI_USER 
GO
ALTER DATABASE [oto] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [oto] SET DB_CHAINING OFF 
GO
ALTER DATABASE [oto] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [oto] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [oto] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [oto] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'oto', N'ON'
GO
ALTER DATABASE [oto] SET QUERY_STORE = ON
GO
ALTER DATABASE [oto] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [oto]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AIRecommendations]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AIRecommendations](
	[RecommendationId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[CarId] [int] NULL,
	[Reason] [nvarchar](500) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[RecommendationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Articles]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Articles](
	[ArticleId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Thumbnail] [nvarchar](max) NULL,
	[Content] [nvarchar](max) NOT NULL,
	[AuthorId] [int] NOT NULL,
	[CreatedAt] [datetime] NULL,
	[IsPublished] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ArticleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Banners]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Banners](
	[BannerId] [int] IDENTITY(1,1) NOT NULL,
	[ImageUrl] [nvarchar](max) NOT NULL,
	[LinkUrl] [nvarchar](max) NULL,
	[Position] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[BannerName] [nvarchar](255) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[BannerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bookings]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bookings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CarId] [int] NOT NULL,
	[CustomerName] [nvarchar](100) NOT NULL,
	[Phone] [varchar](20) NOT NULL,
	[BookingDate] [date] NOT NULL,
	[BookingTime] [varchar](10) NULL,
	[Note] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[UserId] [int] NULL,
	[Status] [nvarchar](50) NOT NULL,
	[ShowroomId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarFeatures]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarFeatures](
	[CarId] [int] NOT NULL,
	[FeatureId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CarId] ASC,
	[FeatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarImages]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarImages](
	[CarImageId] [int] IDENTITY(1,1) NOT NULL,
	[CarId] [int] NULL,
	[ImageUrl] [nvarchar](255) NULL,
	[IsMainImage] [bit] NULL,
	[CreatedAt] [datetime] NULL,
	[Is360Degree] [bit] NOT NULL,
	[ImageType] [nvarchar](max) NULL,
	[FileHash] [nvarchar](max) NULL,
	[Description] [nvarchar](500) NULL,
	[Title] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[CarImageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cars]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cars](
	[CarId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Model] [nvarchar](255) NULL,
	[Year] [int] NULL,
	[Price] [decimal](18, 2) NULL,
	[Color] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[Brand] [nvarchar](100) NULL,
	[Mileage] [decimal](18, 2) NULL,
	[ImageUrl] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[FuelType] [nvarchar](50) NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [int] NULL,
	[Condition] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarSpecifications]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarSpecifications](
	[SpecId] [int] IDENTITY(1,1) NOT NULL,
	[CarId] [int] NOT NULL,
	[Category] [nvarchar](100) NOT NULL,
	[SpecName] [nvarchar](255) NOT NULL,
	[SpecValue] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SpecId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarWishlist]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarWishlist](
	[WishlistId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[CarId] [int] NULL,
	[AddedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[WishlistId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChatMessages]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChatMessages](
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[SessionId] [int] NOT NULL,
	[SenderType] [nvarchar](20) NOT NULL,
	[MessageText] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChatSessions]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChatSessions](
	[SessionId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[GuestToken] [nvarchar](255) NULL,
	[AssignedTo] [int] NULL,
	[Status] [nvarchar](50) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Features]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Features](
	[FeatureId] [int] IDENTITY(1,1) NOT NULL,
	[FeatureName] [nvarchar](255) NOT NULL,
	[Icon] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[FeatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LocationTaxes]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocationTaxes](
	[LocationId] [int] IDENTITY(1,1) NOT NULL,
	[CityName] [nvarchar](100) NOT NULL,
	[RegistrationTaxPercent] [decimal](5, 2) NOT NULL,
	[LicensePlateFee] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[OrderItemId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[CarId] [int] NULL,
	[Quantity] [int] NULL,
	[Price] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[CarId] [int] NULL,
	[OrderDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[PaymentMethod] [nvarchar](50) NULL,
	[ShippingAddress] [nvarchar](500) NULL,
	[PromotionId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentTransactions]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentTransactions](
	[TransactionId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
	[PaymentMethod] [nvarchar](50) NULL,
	[TransactionDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Promotions]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Promotions](
	[PromotionId] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](100) NULL,
	[DiscountPercentage] [decimal](5, 2) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Description] [nvarchar](500) NULL,
	[Status] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[PromotionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reviews]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reviews](
	[ReviewId] [int] IDENTITY(1,1) NOT NULL,
	[CarId] [int] NULL,
	[UserId] [int] NULL,
	[Rating] [int] NULL,
	[Comment] [nvarchar](1000) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Showrooms]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Showrooms](
	[ShowroomId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Address] [nvarchar](500) NOT NULL,
	[Hotline] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ShowroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemAuditLogs]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemAuditLogs](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ActionType] [nvarchar](100) NOT NULL,
	[TableName] [nvarchar](100) NULL,
	[RecordId] [nvarchar](50) NULL,
	[OldValues] [nvarchar](max) NULL,
	[NewValues] [nvarchar](max) NULL,
	[IpAddress] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserActivity]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserActivity](
	[ActivityId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[ActivityType] [nvarchar](50) NULL,
	[CarId] [int] NULL,
	[ActivityDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserLogins]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogins](
	[LoginProvider] [nvarchar](50) NOT NULL,
	[ProviderKey] [nvarchar](255) NOT NULL,
	[UserId] [int] NOT NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 3/5/2026 0:28:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](255) NULL,
	[PasswordHash] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
	[FullName] [nvarchar](255) NULL,
	[Phone] [nvarchar](15) NULL,
	[Role] [nvarchar](50) NOT NULL,
	[Address] [nvarchar](500) NULL,
	[CreatedAt] [datetime] NULL,
	[Status] [nvarchar](50) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [int] NULL,
	[AvatarUrl] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[CarImages] ON 
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1, 1, N'https://example.com/camry_main.jpg', 0, CAST(N'2026-01-22T13:54:59.300' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (2, 1, N'https://example.com/camry_side.jpg', 0, CAST(N'2026-01-22T13:54:59.300' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (3, 2, N'https://example.com/civic_main.jpg', 1, CAST(N'2026-01-22T13:54:59.300' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (4, 2, N'https://example.com/civic_interior.jpg', 0, CAST(N'2026-01-22T13:54:59.300' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (5, 3, N'https://example.com/cx5_main.jpg', 1, CAST(N'2026-01-22T13:54:59.300' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (6, 3, N'https://example.com/cx5_back.jpg', 0, CAST(N'2026-01-22T13:54:59.300' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (7, 4, N'https://example.com/ranger_main.jpg', 1, CAST(N'2026-01-22T13:54:59.300' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (8, 4, N'https://example.com/ranger_offroad.jpg', 0, CAST(N'2026-01-22T13:54:59.300' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (9, 5, N'https://example.com/c300_main.jpg', 1, CAST(N'2026-01-22T13:54:59.300' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (10, 5, N'https://example.com/c300_dashboard.jpg', 0, CAST(N'2026-01-22T13:54:59.300' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (11, 1, N'https://image.danchoioto.vn/tin-tuc/xe-lamborghini/lamborghini-aventador-la.jpg', 0, CAST(N'2026-01-22T14:01:21.920' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (12, 1, N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT9qk18FGUqdzxgIUV5Mrv7D0RC9fdm84NgUg&s', 0, CAST(N'2026-01-22T14:01:21.920' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (13, 2, N'https://example.com/civic_main.jpg', 1, CAST(N'2026-01-22T14:01:21.920' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (14, 2, N'https://example.com/civic_interior.jpg', 0, CAST(N'2026-01-22T14:01:21.920' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (15, 3, N'https://example.com/cx5_main.jpg', 1, CAST(N'2026-01-22T14:01:21.920' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (16, 3, N'https://example.com/cx5_back.jpg', 0, CAST(N'2026-01-22T14:01:21.920' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (17, 4, N'https://example.com/ranger_main.jpg', 1, CAST(N'2026-01-22T14:01:21.920' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (18, 4, N'https://example.com/ranger_offroad.jpg', 0, CAST(N'2026-01-22T14:01:21.920' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (19, 5, N'https://example.com/c300_main.jpg', 1, CAST(N'2026-01-22T14:01:21.920' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (20, 5, N'https://example.com/c300_dashboard.jpg', 0, CAST(N'2026-01-22T14:01:21.920' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (21, 1, N'https://image.danchoioto.vn/tin-tuc/xe-lamborghini/lamborghini-aventador-la.jpg', 1, CAST(N'2026-01-29T15:00:53.593' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (22, 5, N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ8AK1MHeLzMlBwRcqiQLFVU7UddXGo2mpObw&s', 0, CAST(N'2026-01-29T15:08:45.523' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (24, 1024, N'/uploads/Cars/LUX-A/95875036-46c9-4bfe-b4d6-08d67aa80b7e.JPG', 0, CAST(N'2026-02-28T10:26:14.720' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (25, 1024, N'/uploads/Cars\LUX-A\360/exterior/b450aeaf-a59a-4238-9dbc-9cfcc821e75f.JPG', 0, CAST(N'2026-02-28T10:26:32.173' AS DateTime), 1, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (26, 1024, N'/uploads/Cars\LUX-A\360/exterior/c9b2b359-6da6-42e3-95f9-642b5f81aae4.JPG', 0, CAST(N'2026-02-28T10:31:35.693' AS DateTime), 1, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1023, 1024, N'/uploads/Cars/LUX-A/f8f0bc0a-6452-4d29-b6e4-5d4e8e167025.JPG', 0, CAST(N'2026-02-28T21:24:56.617' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1024, 1024, N'/uploads/Cars/LUX-A/3e8c650d-67ba-4cf0-8dd6-f926835f0781.WEBP', 0, CAST(N'2026-02-28T21:25:11.627' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1025, 1024, N'/uploads/Cars/LUX-A/41457859-453b-4ba5-8d0a-fce3666cd83d.WEBP', 0, CAST(N'2026-02-28T21:25:49.577' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1026, 1024, N'/uploads/Cars/LUX-A/609f73f7-0ed7-4535-a31c-d323255b4895.WEBP', 0, CAST(N'2026-02-28T21:25:51.780' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1027, 1024, N'/uploads/Cars/LUX-A/908311a0-7f48-4d6f-b04c-751e381a4b4f.WEBP', 0, CAST(N'2026-02-28T21:26:18.707' AS DateTime), 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1028, 1024, N'/uploads/Cars/LUX-A/29acd0c1-df3c-429e-8109-ce0a559f42f5.JPG', 0, CAST(N'2026-02-28T22:00:19.283' AS DateTime), 0, N'hagshd', N'ab497f81c6d7811776dded1c8bf55793', NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1029, 1024, N'/uploads/Cars/LUX-A/f88a2de5-80c5-4c60-81bc-cb582fd08629.JPG', 0, CAST(N'2026-02-28T22:03:54.610' AS DateTime), 0, N'shin', N'02a11c4cfdccdaf2d1f57a613d7d5b59', NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1030, 1024, N'/uploads/Cars\LUX-A\360/exterior/0ad41c27-544c-4f93-b831-8233b7dbdbd6.JPG', 0, CAST(N'2026-03-01T02:45:00.950' AS DateTime), 1, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1031, 1024, N'/uploads/Cars\LUX-A\360/exterior/8138428f-7aed-4e40-9a38-da5056f6e74d.HEIC', 0, CAST(N'2026-03-01T02:45:00.983' AS DateTime), 1, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1032, 1024, N'/uploads/Cars\LUX-A\360/exterior/896f25e1-7d58-4235-8d4a-c120473864fe.HEIC', 0, CAST(N'2026-03-01T02:45:00.990' AS DateTime), 1, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1033, 1024, N'/uploads/Cars/LUX-A/dbfc4077-2df0-4c11-b9f4-8eb863f4c1d8.HEIC', 0, CAST(N'2026-03-01T02:45:29.497' AS DateTime), 0, N'ádsad', N'f19e1c449c40c13c3728602b4c007045', NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1039, 1024, N'/uploads/Cars/LUX-A/95042c5f-4cac-43a5-8255-7b43054b311f.JPG', 0, CAST(N'2026-03-01T09:59:20.730' AS DateTime), 0, N'ngoại thất', N'038e8eb1a03a627e199a3a57ca344ad5', NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1040, 1024, N'/uploads/Cars/LUX-A/d121218b-fa64-45d1-ab40-ca39dd7c90ab.HEIC', 0, CAST(N'2026-03-01T09:59:20.910' AS DateTime), 0, N'ngoại thất', N'f1de78145b2a86d3687b04f07ea740ad', NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1041, 1024, N'/uploads/Cars/LUX-A/0b57aa74-01d1-4a0a-a0d4-d79a5663c799.JPG', 0, CAST(N'2026-03-01T09:59:20.917' AS DateTime), 0, N'ngoại thất', N'731e3c2e76af4779b1a4ef3431e1b567', NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1042, 1024, N'/uploads/Cars/LUX-A/15fb86f3-afd5-405e-8b62-2a1ba874731d.JPG', 0, CAST(N'2026-03-01T10:00:34.803' AS DateTime), 0, N'ngoại thất', N'4fc3670b4c496ffb560d6f2db40e471f', NULL, NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1043, 1023, N'/uploads/Cars/VF9/5f70eef5-48b5-4718-9e8b-5fa03c5e991d.PNG', 0, CAST(N'2026-03-02T10:21:31.847' AS DateTime), 0, N'lộn xộn', N'58a7af2e89441c79d023024e562361fc', N'thử thách gà máy', N'tft mobile')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [CreatedAt], [Is360Degree], [ImageType], [FileHash], [Description], [Title]) VALUES (1044, 1023, N'/uploads/Cars/VF9/7a20997b-72d1-4b34-bb5f-0f5b752efb9a.HEIC', 0, CAST(N'2026-03-02T10:21:31.970' AS DateTime), 0, N'lộn xộn', N'f1de78145b2a86d3687b04f07ea740ad', N'bầu trời đêm đầy sao', N'ảnh chụp tòa nhà')
GO
SET IDENTITY_INSERT [dbo].[CarImages] OFF
GO
SET IDENTITY_INSERT [dbo].[Cars] ON 
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Condition]) VALUES (1, N'lamobogini', N'Sport', 2026, CAST(50000.00 AS Decimal(18, 2)), N'trắng', N'string', N'lambo', CAST(10000.00 AS Decimal(18, 2)), N'https://giaxeoto.vn/admin/upload/images/resize/640-ngoai-that-xe-lamborghini-sian.jpg', 1, CAST(N'2026-01-22T13:52:15.270' AS DateTime), CAST(N'2026-02-27T21:27:10.343' AS DateTime), NULL, 0, CAST(N'2026-02-27T21:26:58.457' AS DateTime), 1, 0)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Condition]) VALUES (2, N'Honda Civic', N'Sport', 2019, CAST(650000000.00 AS Decimal(18, 2)), N'White', N'phong cách thể thao', N'Honda', CAST(42000.00 AS Decimal(18, 2)), N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ6SzECa2NaZTSkmlf_66zOhpK7V3cVoXQS1Q&s', 1, CAST(N'2026-01-22T13:52:15.270' AS DateTime), CAST(N'2026-02-27T17:02:50.803' AS DateTime), NULL, 0, NULL, NULL, 0)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Condition]) VALUES (3, N'Mazda CX-5', N'Premium', 2021, CAST(900000000.00 AS Decimal(18, 2)), N'Red', N'SUV 5 ch?, n?i th?t cao c?p', N'Mazda', CAST(15000.00 AS Decimal(18, 2)), N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQE9xIfpTo704onWifqv25wfuM2vJrqji0ghg&s', 0, CAST(N'2026-01-22T13:52:15.270' AS DateTime), CAST(N'2026-01-31T08:44:17.360' AS DateTime), NULL, 0, NULL, NULL, 0)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Condition]) VALUES (4, N'Ford Ranger', N'Wildtrak', 2022, CAST(950000000.00 AS Decimal(18, 2)), N'Blue', N'Bán t?i m?nh m?, off-road t?t', N'Ford', CAST(10000.00 AS Decimal(18, 2)), N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTzVsAeejSiIZ7j2FIpwE9xrq-4-dPW8iciuw&s', 0, CAST(N'2026-01-22T13:52:15.270' AS DateTime), CAST(N'2026-01-31T08:44:40.013' AS DateTime), NULL, 0, NULL, NULL, 0)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Condition]) VALUES (5, N'Mercedes-Benz C300', N'AMG', 2021, CAST(1800000000.00 AS Decimal(18, 2)), N'Silver', N'Sedan h?ng sang, hi?u su?t cao', N'Mercedes-Benz', CAST(8000.00 AS Decimal(18, 2)), N'https://mercedes-benz-vn.com/wp-content/uploads/c300.jpg', 0, CAST(N'2026-01-22T13:52:15.270' AS DateTime), CAST(N'2026-01-31T08:45:01.000' AS DateTime), NULL, 0, NULL, NULL, 0)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Condition]) VALUES (11, N'Mẹt', N'sport', 2025, CAST(555555000.00 AS Decimal(18, 2)), N'den', N'adfdfdfd', N'Mẹt', CAST(555555555.00 AS Decimal(18, 2)), N'https://giaxeoto.vn/admin/upload/images/resize/640-ngoai-that-xe-lamborghini-sian.jpg', 0, CAST(N'2026-01-26T03:29:35.470' AS DateTime), CAST(N'2026-01-29T09:06:07.323' AS DateTime), NULL, 0, NULL, NULL, 0)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Condition]) VALUES (21, N'MG', N'Sport', 2025, CAST(800000.00 AS Decimal(18, 2)), N'Đen', N'abc', N'MG', CAST(222.00 AS Decimal(18, 2)), N'https://mglamdong.com/wp-content/uploads/2024/11/khunglogomglamdong1.jpg', 2, CAST(N'2026-01-29T09:08:44.593' AS DateTime), CAST(N'2026-01-29T09:08:44.593' AS DateTime), NULL, 0, NULL, NULL, 0)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Condition]) VALUES (22, N'VF7', N'Sport', 2025, CAST(900000000.00 AS Decimal(18, 2)), N'trắng, xanh, đen, xám', N'là mẫu xe mang phong cách thể thao ', N'Vinfast', CAST(500000.00 AS Decimal(18, 2)), N'/uploads/87f2ce9d-a829-4371-a902-40b5378a880a.JPG', 0, CAST(N'2026-02-25T01:42:40.920' AS DateTime), CAST(N'2026-02-25T01:42:40.920' AS DateTime), N'Điện', 0, NULL, NULL, 0)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Condition]) VALUES (1022, N'VF8', N'Sport', 2025, CAST(130000000.00 AS Decimal(18, 2)), N'đen, trắng , xanh', N'adu vjp lắm nha', N'Vinfast', CAST(500000.00 AS Decimal(18, 2)), N'/uploads/Cars/VF8/84a68aa8-b84e-4e0b-9175-3f0b0a01080f.JPG', 3, CAST(N'2026-02-27T08:55:27.617' AS DateTime), CAST(N'2026-02-28T12:20:09.937' AS DateTime), N'Điện', 0, NULL, NULL, 0)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Condition]) VALUES (1023, N'VF9', N'Stress', 2025, CAST(150000000.00 AS Decimal(18, 2)), N'trắng, lục bảo, đen, xanh', N'sang trọng, quý phái', N'Vinfast', CAST(20000000.00 AS Decimal(18, 2)), N'/uploads/Cars/VF9/7a9f8aba-12e5-436b-b153-f864100d5f9a.JPG', 1, CAST(N'2026-02-27T10:04:51.817' AS DateTime), CAST(N'2026-02-27T10:04:51.817' AS DateTime), N'Điện', 0, NULL, NULL, 0)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Condition]) VALUES (1024, N'LUX-A', N'Sprot', 2020, CAST(40000000.00 AS Decimal(18, 2)), N'trắng, đen, xám , nâu', N'xe cũ nhưng mà trông vẫn ngon nghẻ lắm', N'Vinfast', CAST(30000.00 AS Decimal(18, 2)), N'/uploads/Cars/LUX-A/7a1601fc-072f-4daf-8d10-21208d8687ed.JPG', 1, CAST(N'2026-02-27T10:14:30.517' AS DateTime), CAST(N'2026-02-28T21:24:17.727' AS DateTime), N'Xăng', 0, CAST(N'2026-02-28T12:31:23.643' AS DateTime), 1, 2)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Condition]) VALUES (2027, N'string', N'string', 0, CAST(0.00 AS Decimal(18, 2)), N'string', N'string', N'string', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg', 0, CAST(N'2026-03-03T21:43:55.137' AS DateTime), CAST(N'2026-03-03T21:45:01.590' AS DateTime), N'string', 0, CAST(N'2026-03-03T14:43:36.797' AS DateTime), 0, 2)
GO
SET IDENTITY_INSERT [dbo].[Cars] OFF
GO
SET IDENTITY_INSERT [dbo].[CarWishlist] ON 
GO
INSERT [dbo].[CarWishlist] ([WishlistId], [UserId], [CarId], [AddedAt]) VALUES (6, 1, 2, CAST(N'2026-01-29T16:41:33.050' AS DateTime))
GO
INSERT [dbo].[CarWishlist] ([WishlistId], [UserId], [CarId], [AddedAt]) VALUES (7, 2, 3, CAST(N'2026-01-29T16:41:33.050' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[CarWishlist] OFF
GO
SET IDENTITY_INSERT [dbo].[Features] ON 
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (1, N'Cửa sổ trời', N'fa-sun')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (2, N'Kính không viền', N'fa-window-maximize')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (3, N'Camera 360', N'fa-camera')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (4, N'Hỗ trợ giữ làn', N'fa-road')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (5, N'Ghế sưởi / Làm mát', N'fa-temperature-half')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (1002, N'string', NULL)
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (1003, N'string', NULL)
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (1005, N'loa JBL', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (1006, N'ewrwe', N'/uploads/Features/icon-ewrwe.JPG')
GO
SET IDENTITY_INSERT [dbo].[Features] OFF
GO
SET IDENTITY_INSERT [dbo].[LocationTaxes] ON 
GO
INSERT [dbo].[LocationTaxes] ([LocationId], [CityName], [RegistrationTaxPercent], [LicensePlateFee]) VALUES (1, N'Hà Nội', CAST(12.00 AS Decimal(5, 2)), CAST(20000000.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[LocationTaxes] ([LocationId], [CityName], [RegistrationTaxPercent], [LicensePlateFee]) VALUES (2, N'TP.HCM', CAST(10.00 AS Decimal(5, 2)), CAST(20000000.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[LocationTaxes] ([LocationId], [CityName], [RegistrationTaxPercent], [LicensePlateFee]) VALUES (3, N'Tỉnh thành khác', CAST(10.00 AS Decimal(5, 2)), CAST(2000000.00 AS Decimal(18, 2)))
GO
SET IDENTITY_INSERT [dbo].[LocationTaxes] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderItems] ON 
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price]) VALUES (3, 3, 2, 1, CAST(7000000.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price]) VALUES (4, 4, 2, 1, CAST(650000000.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price]) VALUES (5, NULL, 3, 1, CAST(900000000.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price]) VALUES (6, NULL, 4, 2, CAST(950000000.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price]) VALUES (7, NULL, 5, 1, CAST(1800000000.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price]) VALUES (8, 4, 1, 1, CAST(9000000000.00 AS Decimal(18, 2)))
GO
SET IDENTITY_INSERT [dbo].[OrderItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 
GO
INSERT [dbo].[Orders] ([OrderId], [UserId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId]) VALUES (3, 2, 2, CAST(N'2026-01-22T14:00:38.897' AS DateTime), N'Completed', CAST(10000000000.00 AS Decimal(18, 2)), N'Cash', N'Ship', NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [UserId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId]) VALUES (4, 2, 2, CAST(N'2026-01-22T14:00:38.897' AS DateTime), N'Completed', CAST(650000000.00 AS Decimal(18, 2)), N'Cash', N'456 Hai Bà Trưng, TP.HCM', NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [UserId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId]) VALUES (10, 2, 3, CAST(N'2026-01-29T12:33:24.957' AS DateTime), N'Cancel', CAST(50000000.00 AS Decimal(18, 2)), N'Card', N'Hà Nội', NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [UserId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId]) VALUES (11, 2, 2, CAST(N'2026-01-29T13:13:36.627' AS DateTime), N'Pending', CAST(500000.00 AS Decimal(18, 2)), N'card', N'Ship', NULL)
GO
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[PaymentTransactions] ON 
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (6, 3, CAST(10000000.00 AS Decimal(18, 2)), N'Tiền mặt', CAST(N'2026-01-29T16:15:32.153' AS DateTime), N'Completed')
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (7, 4, CAST(650000000.00 AS Decimal(18, 2)), N'Tiền mặt', CAST(N'2026-01-29T16:15:32.153' AS DateTime), N'Pending')
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (8, 10, CAST(50000000.00 AS Decimal(18, 2)), N'Thẻ', CAST(N'2026-01-29T16:15:32.153' AS DateTime), N'Completed')
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (9, 11, CAST(500000.00 AS Decimal(18, 2)), N'Thẻ', CAST(N'2026-01-29T16:15:32.153' AS DateTime), N'Completed')
GO
SET IDENTITY_INSERT [dbo].[PaymentTransactions] OFF
GO
SET IDENTITY_INSERT [dbo].[Promotions] ON 
GO
INSERT [dbo].[Promotions] ([PromotionId], [Code], [DiscountPercentage], [StartDate], [EndDate], [Description], [Status]) VALUES (1, N'WELCOME2026', CAST(25.00 AS Decimal(5, 2)), CAST(N'2026-01-01T00:00:00.000' AS DateTime), CAST(N'2026-03-31T00:00:00.000' AS DateTime), N'Giảm giá 10% cho khách hàng mới nhân dịp đầu năm', N'Inactive')
GO
INSERT [dbo].[Promotions] ([PromotionId], [Code], [DiscountPercentage], [StartDate], [EndDate], [Description], [Status]) VALUES (2, N'XUAN2026', CAST(5.00 AS Decimal(5, 2)), CAST(N'2026-02-01T00:00:00.000' AS DateTime), CAST(N'2026-02-28T00:00:00.000' AS DateTime), N'Khuyến mãi Tết Nguyên Đán cho tất cả dòng xe', N'Inactive')
GO
INSERT [dbo].[Promotions] ([PromotionId], [Code], [DiscountPercentage], [StartDate], [EndDate], [Description], [Status]) VALUES (3, N'VIPCAR', CAST(15.00 AS Decimal(5, 2)), CAST(N'2026-01-01T00:00:00.000' AS DateTime), CAST(N'2026-12-31T00:00:00.000' AS DateTime), N'Ưu đãi đặc biệt cho khách hàng VIP', N'Active')
GO
INSERT [dbo].[Promotions] ([PromotionId], [Code], [DiscountPercentage], [StartDate], [EndDate], [Description], [Status]) VALUES (4, N'BLACKFRIDAY', CAST(20.00 AS Decimal(5, 2)), CAST(N'2026-11-20T00:00:00.000' AS DateTime), CAST(N'2026-11-30T00:00:00.000' AS DateTime), N'Siêu giảm giá Black Friday', N'Active')
GO
INSERT [dbo].[Promotions] ([PromotionId], [Code], [DiscountPercentage], [StartDate], [EndDate], [Description], [Status]) VALUES (5, N'SUMMERHOT', CAST(8.00 AS Decimal(5, 2)), CAST(N'2026-06-01T00:00:00.000' AS DateTime), CAST(N'2026-08-31T00:00:00.000' AS DateTime), N'Ưu đãi mùa hè sôi động', N'Active')
GO
SET IDENTITY_INSERT [dbo].[Promotions] OFF
GO
SET IDENTITY_INSERT [dbo].[UserActivity] ON 
GO
INSERT [dbo].[UserActivity] ([ActivityId], [UserId], [ActivityType], [CarId], [ActivityDate]) VALUES (2, NULL, N'View', 1, CAST(N'2026-01-22T14:00:04.420' AS DateTime))
GO
INSERT [dbo].[UserActivity] ([ActivityId], [UserId], [ActivityType], [CarId], [ActivityDate]) VALUES (3, NULL, N'Wishlist', 2, CAST(N'2026-01-22T14:00:04.420' AS DateTime))
GO
INSERT [dbo].[UserActivity] ([ActivityId], [UserId], [ActivityType], [CarId], [ActivityDate]) VALUES (4, NULL, N'Purchase', 3, CAST(N'2026-01-22T14:00:04.420' AS DateTime))
GO
INSERT [dbo].[UserActivity] ([ActivityId], [UserId], [ActivityType], [CarId], [ActivityDate]) VALUES (5, NULL, N'View', 4, CAST(N'2026-01-22T14:00:04.420' AS DateTime))
GO
INSERT [dbo].[UserActivity] ([ActivityId], [UserId], [ActivityType], [CarId], [ActivityDate]) VALUES (6, NULL, N'Wishlist', 5, CAST(N'2026-01-22T14:00:04.420' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[UserActivity] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl]) VALUES (1, N'admin01', N'password123', N'admin@example.com', N'Nguyen Van Admin', N'0912345678', N'Admin', N'Ha Noi, Viet Nam', CAST(N'2026-01-29T10:02:58.407' AS DateTime), N'Active', 0, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl]) VALUES (2, N'user01', N'password123', N'khachhang@example.com', N'Tran Thi Khach', N'0987654321', N'Customer', N'HCM, Viet Nam', CAST(N'2026-01-29T10:02:58.407' AS DateTime), N'Active', 0, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Promotio__A25C5AA7A444C1CD]    Script Date: 3/5/2026 0:28:27 ******/
ALTER TABLE [dbo].[Promotions] ADD UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__536C85E4014A9D7C]    Script Date: 3/5/2026 0:28:27 ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D105348B415492]    Script Date: 3/5/2026 0:28:27 ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AIRecommendations] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Articles] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Articles] ADD  DEFAULT ((1)) FOR [IsPublished]
GO
ALTER TABLE [dbo].[Banners] ADD  DEFAULT ((0)) FOR [Position]
GO
ALTER TABLE [dbo].[Banners] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Banners] ADD  DEFAULT ('Khuy?n mãi m?i') FOR [BannerName]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[CarImages] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[CarImages] ADD  DEFAULT ((0)) FOR [Is360Degree]
GO
ALTER TABLE [dbo].[Cars] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Cars] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Cars] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Cars] ADD  DEFAULT ((0)) FOR [Condition]
GO
ALTER TABLE [dbo].[CarWishlist] ADD  DEFAULT (getdate()) FOR [AddedAt]
GO
ALTER TABLE [dbo].[ChatMessages] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ChatSessions] ADD  DEFAULT ('AI_Handling') FOR [Status]
GO
ALTER TABLE [dbo].[ChatSessions] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[OrderItems] ADD  DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[PaymentTransactions] ADD  DEFAULT (getdate()) FOR [TransactionDate]
GO
ALTER TABLE [dbo].[Reviews] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SystemAuditLogs] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[UserActivity] ADD  DEFAULT (getdate()) FOR [ActivityDate]
GO
ALTER TABLE [dbo].[UserLogins] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ('Active') FOR [Status]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[AIRecommendations]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[AIRecommendations]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Articles]  WITH CHECK ADD FOREIGN KEY([AuthorId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD FOREIGN KEY([ShowroomId])
REFERENCES [dbo].[Showrooms] ([ShowroomId])
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[CarFeatures]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[CarFeatures]  WITH CHECK ADD FOREIGN KEY([FeatureId])
REFERENCES [dbo].[Features] ([FeatureId])
GO
ALTER TABLE [dbo].[CarImages]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[CarSpecifications]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[CarWishlist]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[CarWishlist]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ChatMessages]  WITH CHECK ADD FOREIGN KEY([SessionId])
REFERENCES [dbo].[ChatSessions] ([SessionId])
GO
ALTER TABLE [dbo].[ChatSessions]  WITH CHECK ADD FOREIGN KEY([AssignedTo])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ChatSessions]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([PromotionId])
REFERENCES [dbo].[Promotions] ([PromotionId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([PromotionId])
REFERENCES [dbo].[Promotions] ([PromotionId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([PromotionId])
REFERENCES [dbo].[Promotions] ([PromotionId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([PromotionId])
REFERENCES [dbo].[Promotions] ([PromotionId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([PromotionId])
REFERENCES [dbo].[Promotions] ([PromotionId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([PromotionId])
REFERENCES [dbo].[Promotions] ([PromotionId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[PaymentTransactions]  WITH CHECK ADD FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[SystemAuditLogs]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[UserActivity]  WITH CHECK ADD FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[UserActivity]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[UserLogins]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
USE [master]
GO
ALTER DATABASE [oto] SET  READ_WRITE 
GO
