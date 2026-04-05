USE [master]
GO
/****** Object:  Database [oto]    Script Date: 4/5/2026 16:25:51 ******/
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
ALTER DATABASE [oto] SET READ_COMMITTED_SNAPSHOT ON 
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
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 4/5/2026 16:25:51 ******/
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
/****** Object:  Table [dbo].[AIRecommendations]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AIRecommendations](
	[RecommendationId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[CarId] [int] NULL,
	[Reason] [nvarchar](500) NULL,
	[IsHelpful] [bit] NULL,
	[FeedbackNote] [nvarchar](500) NULL,
	[CreatedAt] [datetime] NULL,
 CONSTRAINT [PK__AIRecomm__AA15BEE4630E9EA7] PRIMARY KEY CLUSTERED 
(
	[RecommendationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Articles]    Script Date: 4/5/2026 16:25:52 ******/
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
 CONSTRAINT [PK__Articles__9C6270E8685E14C2] PRIMARY KEY CLUSTERED 
(
	[ArticleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Banners]    Script Date: 4/5/2026 16:25:52 ******/
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
 CONSTRAINT [PK__Banners__32E86AD19CBE788F] PRIMARY KEY CLUSTERED 
(
	[BannerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bookings]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bookings](
	[BookingId] [int] IDENTITY(1,1) NOT NULL,
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
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK__Bookings__3214EC07B3AB1A46] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarFeatures]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarFeatures](
	[CarId] [int] NOT NULL,
	[FeatureId] [int] NOT NULL,
 CONSTRAINT [PK_CarFeatures] PRIMARY KEY CLUSTERED 
(
	[CarId] ASC,
	[FeatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarImages]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarImages](
	[CarImageId] [int] IDENTITY(1,1) NOT NULL,
	[CarId] [int] NULL,
	[ImageUrl] [nvarchar](255) NULL,
	[IsMainImage] [bit] NULL,
	[ImageType] [nvarchar](max) NULL,
	[FileHash] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[Is360Degree] [bit] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
 CONSTRAINT [PK__CarImage__614BE6AFD1BB03D9] PRIMARY KEY CLUSTERED 
(
	[CarImageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarInventories]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarInventories](
	[InventoryId] [int] IDENTITY(1,1) NOT NULL,
	[ShowroomId] [int] NOT NULL,
	[CarId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[DisplayStatus] [nvarchar](50) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_CarInventories] PRIMARY KEY CLUSTERED 
(
	[InventoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cars]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cars](
	[CarId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Model] [nvarchar](255) NULL,
	[Year] [int] NULL,
	[Condition] [int] NOT NULL,
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
	[Transmission] [nvarchar](max) NULL,
	[BodyStyle] [nvarchar](max) NULL,
	[RejectionReason] [nvarchar](max) NULL,
 CONSTRAINT [PK__Cars__68A0342EA480F904] PRIMARY KEY CLUSTERED 
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarSpecifications]    Script Date: 4/5/2026 16:25:52 ******/
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
 CONSTRAINT [PK__CarSpeci__883D567BD7BFC457] PRIMARY KEY CLUSTERED 
(
	[SpecId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarWishlist]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarWishlist](
	[WishlistId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[CarId] [int] NULL,
	[AddedAt] [datetime] NULL,
 CONSTRAINT [PK__CarWishl__233189EB685E97BB] PRIMARY KEY CLUSTERED 
(
	[WishlistId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChatMessages]    Script Date: 4/5/2026 16:25:52 ******/
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
	[IntentLabel] [nvarchar](100) NULL,
 CONSTRAINT [PK__ChatMess__C87C0C9C9F2D92FF] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChatSessions]    Script Date: 4/5/2026 16:25:52 ******/
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
	[LastMessageAt] [datetime2](7) NOT NULL,
	[ShowroomId] [int] NULL,
 CONSTRAINT [PK__ChatSess__C9F49290924C9141] PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Consignments]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Consignments](
	[ConsignmentId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Brand] [nvarchar](100) NOT NULL,
	[Model] [nvarchar](100) NOT NULL,
	[Year] [int] NOT NULL,
	[Mileage] [decimal](18, 2) NOT NULL,
	[ConditionDescription] [nvarchar](1000) NULL,
	[ExpectedPrice] [decimal](18, 2) NULL,
	[AgreedPrice] [decimal](18, 2) NULL,
	[CommissionRate] [decimal](5, 2) NULL,
	[Status] [nvarchar](50) NOT NULL,
	[LinkedCarId] [int] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Consignments] PRIMARY KEY CLUSTERED 
(
	[ConsignmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConsultationProfiles]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConsultationProfiles](
	[ProfileId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[SessionId] [int] NULL,
	[BudgetMin] [decimal](18, 2) NULL,
	[BudgetMax] [decimal](18, 2) NULL,
	[PreferredBodyStyle] [nvarchar](100) NULL,
	[PreferredBrand] [nvarchar](100) NULL,
	[Purpose] [nvarchar](255) NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_ConsultationProfiles] PRIMARY KEY CLUSTERED 
(
	[ProfileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Features]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Features](
	[FeatureId] [int] IDENTITY(1,1) NOT NULL,
	[FeatureName] [nvarchar](255) NOT NULL,
	[Icon] [nvarchar](255) NULL,
 CONSTRAINT [PK__Features__82230BC9B29C0D63] PRIMARY KEY CLUSTERED 
(
	[FeatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LocationTaxes]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocationTaxes](
	[LocationId] [int] IDENTITY(1,1) NOT NULL,
	[CityName] [nvarchar](100) NOT NULL,
	[RegistrationTaxPercent] [decimal](5, 2) NOT NULL,
	[LicensePlateFee] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK__Location__E7FEA49744FC6D16] PRIMARY KEY CLUSTERED 
(
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notifications]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notifications](
	[NotificationId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[ShowroomId] [int] NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Content] [nvarchar](500) NULL,
	[ActionUrl] [nvarchar](255) NULL,
	[IsRead] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[NotificationType] [nvarchar](max) NOT NULL,
	[RoleTarget] [nvarchar](max) NULL,
 CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED 
(
	[NotificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 4/5/2026 16:25:52 ******/
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
 CONSTRAINT [PK__OrderIte__57ED06818F5EAB7E] PRIMARY KEY CLUSTERED 
(
	[OrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 4/5/2026 16:25:52 ******/
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
	[OrderCode] [nvarchar](50) NULL,
	[Subtotal] [decimal](18, 2) NOT NULL,
	[DiscountAmount] [decimal](18, 2) NOT NULL,
	[FinalAmount] [decimal](18, 2) NOT NULL,
	[PaymentStatus] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__Orders__C3905BCFC3E9C951] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentTransactions]    Script Date: 4/5/2026 16:25:52 ******/
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
 CONSTRAINT [PK__PaymentT__55433A6BD6EFEFD7] PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Promotions]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Promotions](
	[PromotionId] [int] IDENTITY(1,1) NOT NULL,
	[PromotionName] [nvarchar](255) NULL,
	[DiscountAmount] [decimal](18, 2) NULL,
	[Code] [nvarchar](100) NULL,
	[DiscountPercentage] [decimal](5, 2) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Description] [nvarchar](500) NULL,
	[Status] [nvarchar](50) NULL,
 CONSTRAINT [PK__Promotio__52C42FCF8A6988C7] PRIMARY KEY CLUSTERED 
(
	[PromotionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reviews]    Script Date: 4/5/2026 16:25:52 ******/
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
 CONSTRAINT [PK__Reviews__74BC79CE36435DC2] PRIMARY KEY CLUSTERED 
(
	[ReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Showrooms]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Showrooms](
	[ShowroomId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Province] [nvarchar](100) NOT NULL,
	[District] [nvarchar](100) NOT NULL,
	[StreetAddress] [nvarchar](300) NOT NULL,
	[Hotline] [nvarchar](20) NULL,
 CONSTRAINT [PK__Showroom__A7726CBBA913B30F] PRIMARY KEY CLUSTERED 
(
	[ShowroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemAuditLogs]    Script Date: 4/5/2026 16:25:52 ******/
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
 CONSTRAINT [PK__SystemAu__5E548648E69FCEBF] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserActivity]    Script Date: 4/5/2026 16:25:52 ******/
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
 CONSTRAINT [PK__UserActi__45F4A79164816C2C] PRIMARY KEY CLUSTERED 
(
	[ActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserLogins]    Script Date: 4/5/2026 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogins](
	[LoginProvider] [nvarchar](50) NOT NULL,
	[ProviderKey] [nvarchar](255) NOT NULL,
	[UserId] [int] NOT NULL,
	[CreatedAt] [datetime] NULL,
 CONSTRAINT [PK__UserLogi__2B2C5B522D37D021] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 4/5/2026 16:25:52 ******/
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
	[ShowroomId] [int] NULL,
 CONSTRAINT [PK__Users__1788CC4C5DD76572] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260319183604_InitialCreate', N'8.0.24')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260320065758_AddRejectionReasonToCar', N'8.0.24')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260324170001_FinalFixBookingTable', N'8.0.24')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260325160433_AddNotificationTable', N'8.0.24')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260325162458_AddNotificationTypeColumn', N'8.0.24')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260405071645_AddRoleTargetColumn', N'8.0.24')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260405080002_AddShowroomIdToChatSession', N'8.0.24')
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (1, 1)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (8, 1)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (1, 2)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (1, 3)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (6, 3)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (8, 3)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (1, 4)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (8, 5)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (6, 6)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (7, 8)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (6, 9)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (7, 9)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (7, 10)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (8, 10)
GO
SET IDENTITY_INSERT [dbo].[CarInventories] ON 
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (1, 1, 1, 5, N'OnDisplay', CAST(N'2026-03-20T01:55:38.0497841' AS DateTime2))
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (4, 4, 6, 1, N'Available', CAST(N'2026-03-20T13:16:40.2294239' AS DateTime2))
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (5, 2, 7, 0, N'Out of stock', CAST(N'2026-03-20T13:08:49.6508395' AS DateTime2))
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (6, 1, 6, 1, N'Available', CAST(N'2026-03-20T13:16:34.4679323' AS DateTime2))
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (1006, 2, 8, 5, N'Pending', CAST(N'2026-04-05T13:07:08.8650543' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[CarInventories] OFF
GO
SET IDENTITY_INSERT [dbo].[Cars] ON 
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (1, N'VF 8 Plus', N'SUV Electric', 2024, 0, CAST(1270000000.00 AS Decimal(18, 2)), N'Trắng Trân Châu', N'Mẫu SUV điện thông minh hạng D với công nghệ hỗ trợ lái ADAS.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg.png', 1, CAST(N'2026-03-20T01:55:37.810' AS DateTime), CAST(N'2026-03-20T01:55:37.810' AS DateTime), N'Điện', 0, NULL, NULL, N'Số tự động', N'SUV', NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (6, N'Ranger Wildtrak 4.0L 4x4', N'Next-Gen Wildtrak', 2024, 0, CAST(979000000.00 AS Decimal(18, 2)), N'Cam Luxe', N'Mẫu bán tải đa năng nhất thế giới. Kết hợp hoàn hảo giữa sức mạnh vận hành và công nghệ thông minh hàng đầu.', N'FORD', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg.png', 1, CAST(N'2026-03-20T02:15:25.870' AS DateTime), CAST(N'2026-03-20T02:15:25.870' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'Bán tải', NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (7, N'Xpander Premium', N'MPV 7 chỗ', 2024, 0, CAST(658000000.00 AS Decimal(18, 2)), N'Trắng', N'Mẫu MPV 7 chỗ bán chạy nhất Việt Nam. Thiết kế Dynamic Shield hiện đại, khoảng sáng gầm cực cao (225mm) chấp mọi cung đường ngập lụt.', N'MITSUBISHI', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg.png', 0, CAST(N'2026-03-20T02:31:12.173' AS DateTime), CAST(N'2026-03-20T02:31:12.173' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'MPV', NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (8, N'Toyota Alphard', N'2024 Executive Lounge', 2024, 0, CAST(4500000000.00 AS Decimal(18, 2)), N'Trắng Ngọc Trai', N'Xe mới 100%, nhập khẩu nguyên chiếc', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg.png', 1, CAST(N'2026-04-05T13:07:08.767' AS DateTime), CAST(N'2026-04-05T13:07:08.767' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'MPV', NULL)
GO
SET IDENTITY_INSERT [dbo].[Cars] OFF
GO
SET IDENTITY_INSERT [dbo].[CarSpecifications] ON 
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (1, 1, N'Động cơ', N'Công suất tối đa', N'402 hp')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (2, 1, N'Động cơ', N'Mô-men xoắn', N'620 Nm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (3, 1, N'Pin', N'Quãng đường di chuyển', N'400 km')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (4, 1, N'An toàn', N'Túi khí', N'11 túi khí')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (5, 1, N'Ngoại thất', N'Mâm xe', N'20 inch')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (6, 6, N'Động cơ', N'Loại động cơ', N'Bi-Turbo Diesel 2.0L')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (7, 6, N'Động cơ', N'Công suất tối đa', N'210 hp')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (8, 6, N'Vận hành', N'Hệ dẫn động', N'2 cầu (4x4)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (9, 6, N'Vận hành', N'Khả năng lội nước', N'800 mm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (10, 6, N'Công nghệ', N'Màn hình trung tâm', N'12 inch đặt dọc')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (11, 6, N'An toàn', N'Hệ thống phanh', N'ABS & EBD & ESP')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (12, 7, N'Động cơ', N'Loại động cơ', N'1.5L MIVEC')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (13, 7, N'Động cơ', N'Công suất tối đa', N'104 hp')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (14, 7, N'Nội thất', N'Số chỗ ngồi', N'7 chỗ')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (15, 7, N'Vận hành', N'Khoảng sáng gầm', N'225 mm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (16, 7, N'An toàn', N'Hệ thống phanh', N'ABS, EBD, BA')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (17, 7, N'An toàn', N'Cân bằng điện tử', N'ASC')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (18, 7, N'Tiện nghi', N'Phanh tay', N'Điện tử & Auto Hold')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (19, 8, N'Động cơ', N'Mã lực', N'300 HP')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (20, 8, N'Kích thước', N'Chiều dài', N'4940 mm')
GO
SET IDENTITY_INSERT [dbo].[CarSpecifications] OFF
GO
SET IDENTITY_INSERT [dbo].[CarWishlist] ON 
GO
INSERT [dbo].[CarWishlist] ([WishlistId], [UserId], [CarId], [AddedAt]) VALUES (1, 1, 1, CAST(N'2026-03-26T01:01:06.093' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[CarWishlist] OFF
GO
SET IDENTITY_INSERT [dbo].[Features] ON 
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (1, N'Hệ thống ABS', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (2, N'Camera 360', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (3, N'Cảnh báo điểm mù', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (4, N'Cảm biến lùi', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (5, N'Hỗ trợ giữ làn', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (6, N'7 Túi khí', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (7, N'Apple CarPlay', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (8, N'Sạc không dây', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (9, N'Màn hình HUD', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (10, N'Loa Bose', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (11, N'Điều khiển giọng nói', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (12, N'Cửa sổ trời Toàn cảnh', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (13, N'Ghế da Nappa', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (14, N'Ghế chỉnh điện', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (15, N'Cốp điện', N'/uploads/Features/default-feature.png')
GO
SET IDENTITY_INSERT [dbo].[Features] OFF
GO
SET IDENTITY_INSERT [dbo].[Notifications] ON 
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (1, 1, NULL, N'Chào mừng bạn gia nhập! 🎉', N'Xin chào Lê Quang Vinh, chúc bạn sớm tìm được chiếc xe ưng ý tại Showroom của chúng tôi!', N'/', 0, CAST(N'2026-03-26T00:51:31.2021853' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (2, 2, NULL, N'Chào mừng bạn gia nhập! 🎉', N'Xin chào Vinh, chúc bạn sớm tìm được chiếc xe ưng ý tại Showroom của chúng tôi!', N'/', 0, CAST(N'2026-03-27T22:06:28.1272184' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (3, 3, NULL, N'Chào mừng bạn gia nhập! 🎉', N'Xin chào Lê Quang Vinh 1, chúc bạn sớm tìm được chiếc xe ưng ý tại Showroom của chúng tôi!', N'/', 0, CAST(N'2026-03-27T22:07:52.9348424' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (4, 4, NULL, N'Chào mừng bạn gia nhập! 🎉', N'Xin chào Lê Quang Vinh 2, chúc bạn sớm tìm được chiếc xe ưng ý tại Showroom của chúng tôi!', N'/', 0, CAST(N'2026-03-27T22:08:10.6445927' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (5, 5, NULL, N'Chào mừng bạn gia nhập! 🎉', N'Xin chào Lê Quang Vinh 2, chúc bạn sớm tìm được chiếc xe ưng ý tại Showroom của chúng tôi!', N'/', 0, CAST(N'2026-03-27T22:08:18.8582391' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (6, NULL, 1, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng ShowroomManager1 vừa được cấp tài khoản ShowroomManager tại chi nhánh chúng ta.', N'/admin/users', 0, CAST(N'2026-03-27T22:12:26.7220925' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (7, NULL, 1, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng ShowroomSales1 vừa được cấp tài khoản ShowroomSales tại chi nhánh chúng ta.', N'/admin/users', 0, CAST(N'2026-03-27T22:13:48.7313122' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (8, 1, NULL, N'Thay đổi trạng thái tài khoản ⚠️', N'Tài khoản của bạn vừa được khôi phục hoạt động bởi Admin hệ thống.', N'#', 0, CAST(N'2026-03-27T22:17:23.7468256' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (9, 8, NULL, N'Thay đổi trạng thái tài khoản ⚠️', N'Tài khoản của bạn vừa được khôi phục hoạt động bởi Admin hệ thống.', N'#', 0, CAST(N'2026-03-27T22:18:32.5360151' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (10, 8, NULL, N'Thay đổi trạng thái tài khoản ⚠️', N'Tài khoản của bạn vừa được khôi phục hoạt động bởi Admin hệ thống.', N'#', 0, CAST(N'2026-03-27T22:18:34.5268640' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (11, 8, NULL, N'Thay đổi trạng thái tài khoản ⚠️', N'Tài khoản của bạn vừa được khôi phục hoạt động bởi Admin hệ thống.', N'#', 0, CAST(N'2026-03-27T22:18:35.0352185' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (12, 8, NULL, N'Thay đổi trạng thái tài khoản ⚠️', N'Tài khoản của bạn vừa được khôi phục hoạt động bởi Admin hệ thống.', N'#', 0, CAST(N'2026-03-27T22:18:35.1958374' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (13, 8, NULL, N'Thay đổi trạng thái tài khoản ⚠️', N'Tài khoản của bạn vừa được khôi phục hoạt động bởi Admin hệ thống.', N'#', 0, CAST(N'2026-03-27T22:18:35.5148796' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (14, 8, NULL, N'Thay đổi trạng thái tài khoản ⚠️', N'Tài khoản của bạn vừa được khôi phục hoạt động bởi Admin hệ thống.', N'#', 0, CAST(N'2026-03-27T22:18:35.6672835' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (15, 8, NULL, N'Thay đổi trạng thái tài khoản ⚠️', N'Tài khoản của bạn vừa được khôi phục hoạt động bởi Admin hệ thống.', N'#', 0, CAST(N'2026-03-27T22:18:35.8285801' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (16, 8, NULL, N'Thay đổi trạng thái tài khoản ⚠️', N'Tài khoản của bạn vừa được khôi phục hoạt động bởi Admin hệ thống.', N'#', 0, CAST(N'2026-03-27T22:18:35.9754897' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (17, 8, NULL, N'Thay đổi trạng thái tài khoản ⚠️', N'Tài khoản của bạn vừa bị khóa bởi Admin hệ thống.', N'#', 0, CAST(N'2026-03-27T22:19:12.8702912' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (18, 8, NULL, N'Thay đổi trạng thái tài khoản ⚠️', N'Tài khoản của bạn vừa được khôi phục hoạt động bởi Admin hệ thống.', N'#', 0, CAST(N'2026-03-27T22:19:19.0785339' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (19, 8, NULL, N'Thay đổi trạng thái tài khoản ⚠️', N'Tài khoản của bạn vừa được khôi phục hoạt động bởi Admin hệ thống.', N'#', 0, CAST(N'2026-03-27T22:19:25.8289528' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (20, NULL, 2, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Nguyễn Văn Quản Lý vừa được cấp tài khoản ShowroomManager tại chi nhánh chúng ta.', N'/admin/users', 0, CAST(N'2026-03-27T23:37:44.0026285' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (21, NULL, 2, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Lê Văn Sales Một vừa được cấp tài khoản ShowroomSales tại chi nhánh chúng ta.', N'/admin/users', 0, CAST(N'2026-03-27T23:37:52.1379372' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (22, NULL, 2, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Trần Thị Sales Hai vừa được cấp tài khoản ShowroomSales tại chi nhánh chúng ta.', N'/admin/users', 0, CAST(N'2026-03-27T23:37:58.0477698' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (23, NULL, 2, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Phạm Văn Sales Ba vừa được cấp tài khoản ShowroomSales tại chi nhánh chúng ta.', N'/admin/users', 0, CAST(N'2026-03-27T23:38:41.0041867' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (24, NULL, 2, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Hoàng Thị Sales Bốn vừa được cấp tài khoản ShowroomSales tại chi nhánh chúng ta.', N'/admin/users', 0, CAST(N'2026-03-27T23:38:48.9716757' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (25, NULL, 2, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Đặng Văn Sales Năm vừa được cấp tài khoản ShowroomSales tại chi nhánh chúng ta.', N'/admin/users', 0, CAST(N'2026-03-27T23:38:54.6944600' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (26, 1, NULL, N'Thay đổi trạng thái tài khoản ⚠️', N'Tài khoản của bạn vừa được khôi phục hoạt động bởi Admin hệ thống.', N'#', 0, CAST(N'2026-03-28T00:02:53.5019534' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (27, NULL, 2, N'Tài khoản bị Xóa ❌', N'Admin vừa gạch tên Đặng Văn Sales Năm (ShowroomSales) khỏi hệ thống chi nhánh này.', N'/admin/users', 0, CAST(N'2026-03-28T00:11:46.0411487' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (28, 17, NULL, N'Chào mừng bạn gia nhập! 🎉', N'Xin chào Lê Quang Vinh, chúc bạn sớm tìm được chiếc xe ưng ý tại Showroom của chúng tôi!', N'/', 0, CAST(N'2026-03-28T23:55:52.6863664' AS DateTime2), N'System', NULL)
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [IsRead], [CreatedAt], [NotificationType], [RoleTarget]) VALUES (1028, NULL, 2, N'Có xe mới cần duyệt', N'Nhân viên vừa đăng mẫu TOYOTA Toyota Alphard. Sếp vào duyệt nhé!', N'/admin/cars/approve/8', 0, CAST(N'2026-04-05T13:07:08.9397208' AS DateTime2), N'CarApproval', NULL)
GO
SET IDENTITY_INSERT [dbo].[Notifications] OFF
GO
SET IDENTITY_INSERT [dbo].[Showrooms] ON 
GO
INSERT [dbo].[Showrooms] ([ShowroomId], [Name], [Province], [District], [StreetAddress], [Hotline]) VALUES (1, N'VinFast Thăng Long', N'Hà Nội', N'Cầu Giấy', N'Số 68, Đường Trịnh Văn Bô', N'1900 232323')
GO
INSERT [dbo].[Showrooms] ([ShowroomId], [Name], [Province], [District], [StreetAddress], [Hotline]) VALUES (2, N'Showroom Sài Gòn Center', N'TP. HCM', N'Quận 1', N'Số 1, Lê Duẩn, Phường Bến Nghé', N'0908 123 456')
GO
INSERT [dbo].[Showrooms] ([ShowroomId], [Name], [Province], [District], [StreetAddress], [Hotline]) VALUES (3, N'Auto Đà Nẵng Riverside', N'Đà Nẵng', N'Hải Châu', N'Số 50, Bạch Đằng', N'0236 3888 999')
GO
INSERT [dbo].[Showrooms] ([ShowroomId], [Name], [Province], [District], [StreetAddress], [Hotline]) VALUES (4, N'Showroom Đất Cảng', N'Hải Phòng', N'Lê Chân', N'Số 12, Lạch Tray', N'0225 3666 777')
GO
SET IDENTITY_INSERT [dbo].[Showrooms] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (1, N'wwangvinh04', N'admin', N'wwangvinh04@gmail.com', N'Lê Quang Vinh', N'0965346160', N'Admin', NULL, CAST(N'2026-03-26T00:51:31.060' AS DateTime), N'Inactive', 1, CAST(N'2026-03-28T00:11:10.583' AS DateTime), 2, NULL, NULL)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (2, N'admin1', N'$2a$11$3XoEOqVO8L.ee/TFk14w3.6dS4rg0RBpe2ikwL7gke4PZOuONuH2a', N'ww', N'Vinh', N'11111', N'Admin', NULL, CAST(N'2026-03-27T22:06:27.960' AS DateTime), N'Active', 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (3, N'wwangvinh05', N'$2a$11$YSIyD2e.Nzyzfop9L5.BEe9x.mvqedKmB6qWPUGkZREe/TF5qk.zu', N'sad', N'Lê Quang Vinh 1', N'1231414', N'Customer', NULL, CAST(N'2026-03-27T22:07:52.927' AS DateTime), N'Active', 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (4, N'wwangvinh06', N'$2a$11$RJNEvbCLZ4.56aHQyxGIC.pRRLo/LvQsIoiBepSnws6CUa8jhadSG', N'saed', N'Lê Quang Vinh 2', N'1231414', N'Customer', NULL, CAST(N'2026-03-27T22:08:10.633' AS DateTime), N'Active', 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (5, N'wwangvinh07', N'$2a$11$D1m8pOzZzuURTgBqlEND7OKhtTT2DdbeombPY87UM9drkae5tX89.', N'safed', N'Lê Quang Vinh 2', N'1231414', N'Customer', NULL, CAST(N'2026-03-27T22:08:18.850' AS DateTime), N'Active', 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (6, N'ShowroomManager1', N'$2a$11$w.Y1vODMPXFM4dqnFElHNOVsQz/lPhN1gMW2WFZupiccrrEdAsN4y', N'ShowroomManager1', N'ShowroomManager1', N'0990332677', N'ShowroomManager', NULL, CAST(N'2026-03-27T22:12:26.707' AS DateTime), N'Active', 0, NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (8, N'ShowroomSales1', N'$2a$11$/p/fb/yj5CxfkABvn1D74OO.PCRQ45uydqNnPspSVCqFsB2nliZsq', N'string', N'ShowroomSales1', N'71092783907', N'ShowroomSales', NULL, CAST(N'2026-03-27T22:13:48.723' AS DateTime), N'Active', 0, NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (11, N'manager_sr2', N'$2a$11$v.Q3yQzVTR1RIYpW/v4YC.8V1B9E9TETA6LkhgFIW7vQnudq2SwVm', N'manager.sr2@oto.com', N'Nguyễn Văn Quản Lý', N'0911000001', N'ShowroomManager', NULL, CAST(N'2026-03-27T23:37:43.897' AS DateTime), N'Active', 0, NULL, NULL, NULL, 2)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (12, N'sales1_sr2', N'$2a$11$mdYgPIYcoZYWAG8NluUbSeI2nCDAwlqeyNPMwvPj5t2UMg17udpRy', N'sales1.sr2@oto.com', N'Lê Văn Sales Một', N'0911000002', N'ShowroomSales', NULL, CAST(N'2026-03-27T23:37:52.133' AS DateTime), N'Active', 0, NULL, NULL, NULL, 2)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (13, N'sales2_sr2', N'$2a$11$IsdNnNSitRR/OM85HJNXO.vKQmu42d8eDD64ToDkzrXlWwDyV.TWq', N'sales2.sr2@oto.com', N'Trần Thị Sales Hai', N'0911000003', N'ShowroomSales', NULL, CAST(N'2026-03-27T23:37:58.043' AS DateTime), N'Active', 0, NULL, NULL, NULL, 2)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (14, N'sales3_sr2', N'$2a$11$jNoMSkYy2naka5Ymjmj6/OEVMbewv/Xts57IYGFft/UJEzvBVPlX.', N'sales3.sr2@oto.com', N'Phạm Văn Sales Ba', N'0911000004', N'ShowroomSales', NULL, CAST(N'2026-03-27T23:38:40.997' AS DateTime), N'Active', 0, NULL, NULL, NULL, 2)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (15, N'sales4_sr2', N'$2a$11$arVagfOsCAJffhPDe8Jc.OpG2CGYJ.DcX5FDewONtdjubW62eBsd6', N'sales4.sr2@oto.com', N'Hoàng Thị Sales Bốn', N'0911000005', N'ShowroomSales', NULL, CAST(N'2026-03-27T23:38:48.967' AS DateTime), N'Active', 0, NULL, NULL, NULL, 2)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (16, N'sales5_sr2', N'$2a$11$49LwNy7V6wz2RMb0GA0ayOp2ghMxDqBow3/X8OFMWbaon/NIbv7VW', N'sales5.sr2@oto.com', N'Đặng Văn Sales Năm', N'0911000006', N'ShowroomSales', NULL, CAST(N'2026-03-27T23:38:54.690' AS DateTime), N'Inactive', 1, CAST(N'2026-03-28T00:11:46.030' AS DateTime), 2, NULL, 2)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (17, N'wwangvinh004', N'$2a$11$dOMP9/XW94N1DYqL7rwXdulqL9clTxSCcwY/M01n3PMhtZyvZ63KO', N'wwangvinh004@gmail.com', N'Lê Quang Vinh', N'0965346160', N'Customer', NULL, CAST(N'2026-03-28T23:55:52.570' AS DateTime), N'Active', 0, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
/****** Object:  Index [IX_AIRecommendations_CarId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_AIRecommendations_CarId] ON [dbo].[AIRecommendations]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AIRecommendations_UserId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_AIRecommendations_UserId] ON [dbo].[AIRecommendations]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Articles_AuthorId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Articles_AuthorId] ON [dbo].[Articles]
(
	[AuthorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_CarId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_CarId] ON [dbo].[Bookings]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_ShowroomId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_ShowroomId] ON [dbo].[Bookings]
(
	[ShowroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_UserId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_UserId] ON [dbo].[Bookings]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarFeatures_FeatureId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_CarFeatures_FeatureId] ON [dbo].[CarFeatures]
(
	[FeatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarImages_CarId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_CarImages_CarId] ON [dbo].[CarImages]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarInventories_CarId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_CarInventories_CarId] ON [dbo].[CarInventories]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarInventories_ShowroomId_CarId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_CarInventories_ShowroomId_CarId] ON [dbo].[CarInventories]
(
	[ShowroomId] ASC,
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarSpecifications_CarId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_CarSpecifications_CarId] ON [dbo].[CarSpecifications]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarWishlist_CarId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_CarWishlist_CarId] ON [dbo].[CarWishlist]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarWishlist_UserId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_CarWishlist_UserId] ON [dbo].[CarWishlist]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChatMessages_SessionId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_ChatMessages_SessionId] ON [dbo].[ChatMessages]
(
	[SessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChatSessions_AssignedTo]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_ChatSessions_AssignedTo] ON [dbo].[ChatSessions]
(
	[AssignedTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChatSessions_UserId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_ChatSessions_UserId] ON [dbo].[ChatSessions]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Consignments_LinkedCarId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Consignments_LinkedCarId] ON [dbo].[Consignments]
(
	[LinkedCarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Consignments_UserId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Consignments_UserId] ON [dbo].[Consignments]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConsultationProfiles_SessionId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_ConsultationProfiles_SessionId] ON [dbo].[ConsultationProfiles]
(
	[SessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConsultationProfiles_UserId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_ConsultationProfiles_UserId] ON [dbo].[ConsultationProfiles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Notifications_ShowroomId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Notifications_ShowroomId] ON [dbo].[Notifications]
(
	[ShowroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Notifications_UserId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Notifications_UserId] ON [dbo].[Notifications]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_CarId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_CarId] ON [dbo].[OrderItems]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_OrderId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderId] ON [dbo].[OrderItems]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_CarId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Orders_CarId] ON [dbo].[Orders]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_PromotionId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Orders_PromotionId] ON [dbo].[Orders]
(
	[PromotionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_UserId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Orders_UserId] ON [dbo].[Orders]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PaymentTransactions_OrderId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_PaymentTransactions_OrderId] ON [dbo].[PaymentTransactions]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Promotio__A25C5AA7A444C1CD]    Script Date: 4/5/2026 16:25:52 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Promotio__A25C5AA7A444C1CD] ON [dbo].[Promotions]
(
	[Code] ASC
)
WHERE ([Code] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Reviews_CarId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Reviews_CarId] ON [dbo].[Reviews]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Reviews_UserId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Reviews_UserId] ON [dbo].[Reviews]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SystemAuditLogs_UserId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_SystemAuditLogs_UserId] ON [dbo].[SystemAuditLogs]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserActivity_CarId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_UserActivity_CarId] ON [dbo].[UserActivity]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserActivity_UserId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_UserActivity_UserId] ON [dbo].[UserActivity]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserLogins_UserId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_UserLogins_UserId] ON [dbo].[UserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Users_ShowroomId]    Script Date: 4/5/2026 16:25:52 ******/
CREATE NONCLUSTERED INDEX [IX_Users_ShowroomId] ON [dbo].[Users]
(
	[ShowroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__536C85E4014A9D7C]    Script Date: 4/5/2026 16:25:52 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Users__536C85E4014A9D7C] ON [dbo].[Users]
(
	[Username] ASC
)
WHERE ([Username] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D105348B415492]    Script Date: 4/5/2026 16:25:52 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Users__A9D105348B415492] ON [dbo].[Users]
(
	[Email] ASC
)
WHERE ([Email] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AIRecommendations] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Articles] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Articles] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsPublished]
GO
ALTER TABLE [dbo].[Banners] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[Banners] ADD  DEFAULT (N'Khuy?n mãi m?i') FOR [BannerName]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT (N'Pending') FOR [Status]
GO
ALTER TABLE [dbo].[CarImages] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Cars] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Cars] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[CarWishlist] ADD  DEFAULT (getdate()) FOR [AddedAt]
GO
ALTER TABLE [dbo].[ChatMessages] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ChatSessions] ADD  DEFAULT (N'AI_Handling') FOR [Status]
GO
ALTER TABLE [dbo].[ChatSessions] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ChatSessions] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [LastMessageAt]
GO
ALTER TABLE [dbo].[Notifications] ADD  DEFAULT (N'') FOR [NotificationType]
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
ALTER TABLE [dbo].[Users] ADD  DEFAULT (N'Active') FOR [Status]
GO
ALTER TABLE [dbo].[AIRecommendations]  WITH CHECK ADD  CONSTRAINT [FK__AIRecomme__CarId__5AEE82B9] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[AIRecommendations] CHECK CONSTRAINT [FK__AIRecomme__CarId__5AEE82B9]
GO
ALTER TABLE [dbo].[AIRecommendations]  WITH CHECK ADD  CONSTRAINT [FK__AIRecomme__UserI__5BE2A6F2] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[AIRecommendations] CHECK CONSTRAINT [FK__AIRecomme__UserI__5BE2A6F2]
GO
ALTER TABLE [dbo].[Articles]  WITH CHECK ADD  CONSTRAINT [FK__Articles__Author__41EDCAC5] FOREIGN KEY([AuthorId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Articles] CHECK CONSTRAINT [FK__Articles__Author__41EDCAC5]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK__Bookings__CarId__2739D489] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK__Bookings__CarId__2739D489]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK__Bookings__Showro__3D2915A8] FOREIGN KEY([ShowroomId])
REFERENCES [dbo].[Showrooms] ([ShowroomId])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK__Bookings__Showro__3D2915A8]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK__Bookings__UserId__282DF8C2] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK__Bookings__UserId__282DF8C2]
GO
ALTER TABLE [dbo].[CarFeatures]  WITH CHECK ADD  CONSTRAINT [FK_CarFeatures_Cars_CarId] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CarFeatures] CHECK CONSTRAINT [FK_CarFeatures_Cars_CarId]
GO
ALTER TABLE [dbo].[CarFeatures]  WITH CHECK ADD  CONSTRAINT [FK_CarFeatures_Features_FeatureId] FOREIGN KEY([FeatureId])
REFERENCES [dbo].[Features] ([FeatureId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CarFeatures] CHECK CONSTRAINT [FK_CarFeatures_Features_FeatureId]
GO
ALTER TABLE [dbo].[CarImages]  WITH CHECK ADD  CONSTRAINT [FK__CarImages__CarId__5CD6CB2B] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[CarImages] CHECK CONSTRAINT [FK__CarImages__CarId__5CD6CB2B]
GO
ALTER TABLE [dbo].[CarInventories]  WITH CHECK ADD  CONSTRAINT [FK_CarInventories_Cars_CarId] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CarInventories] CHECK CONSTRAINT [FK_CarInventories_Cars_CarId]
GO
ALTER TABLE [dbo].[CarInventories]  WITH CHECK ADD  CONSTRAINT [FK_CarInventories_Showrooms_ShowroomId] FOREIGN KEY([ShowroomId])
REFERENCES [dbo].[Showrooms] ([ShowroomId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CarInventories] CHECK CONSTRAINT [FK_CarInventories_Showrooms_ShowroomId]
GO
ALTER TABLE [dbo].[CarSpecifications]  WITH CHECK ADD  CONSTRAINT [FK__CarSpecif__CarId__06CD04F7] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[CarSpecifications] CHECK CONSTRAINT [FK__CarSpecif__CarId__06CD04F7]
GO
ALTER TABLE [dbo].[CarWishlist]  WITH CHECK ADD  CONSTRAINT [FK__CarWishli__CarId__5DCAEF64] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[CarWishlist] CHECK CONSTRAINT [FK__CarWishli__CarId__5DCAEF64]
GO
ALTER TABLE [dbo].[CarWishlist]  WITH CHECK ADD  CONSTRAINT [FK__CarWishli__UserI__5EBF139D] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[CarWishlist] CHECK CONSTRAINT [FK__CarWishli__UserI__5EBF139D]
GO
ALTER TABLE [dbo].[ChatMessages]  WITH CHECK ADD  CONSTRAINT [FK__ChatMessa__Sessi__160F4887] FOREIGN KEY([SessionId])
REFERENCES [dbo].[ChatSessions] ([SessionId])
GO
ALTER TABLE [dbo].[ChatMessages] CHECK CONSTRAINT [FK__ChatMessa__Sessi__160F4887]
GO
ALTER TABLE [dbo].[ChatSessions]  WITH CHECK ADD  CONSTRAINT [FK__ChatSessi__Assig__123EB7A3] FOREIGN KEY([AssignedTo])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ChatSessions] CHECK CONSTRAINT [FK__ChatSessi__Assig__123EB7A3]
GO
ALTER TABLE [dbo].[ChatSessions]  WITH CHECK ADD  CONSTRAINT [FK__ChatSessi__UserI__114A936A] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ChatSessions] CHECK CONSTRAINT [FK__ChatSessi__UserI__114A936A]
GO
ALTER TABLE [dbo].[Consignments]  WITH CHECK ADD  CONSTRAINT [FK_Consignments_Cars_LinkedCarId] FOREIGN KEY([LinkedCarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[Consignments] CHECK CONSTRAINT [FK_Consignments_Cars_LinkedCarId]
GO
ALTER TABLE [dbo].[Consignments]  WITH CHECK ADD  CONSTRAINT [FK_Consignments_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Consignments] CHECK CONSTRAINT [FK_Consignments_Users_UserId]
GO
ALTER TABLE [dbo].[ConsultationProfiles]  WITH CHECK ADD  CONSTRAINT [FK_ConsultationProfiles_ChatSessions_SessionId] FOREIGN KEY([SessionId])
REFERENCES [dbo].[ChatSessions] ([SessionId])
GO
ALTER TABLE [dbo].[ConsultationProfiles] CHECK CONSTRAINT [FK_ConsultationProfiles_ChatSessions_SessionId]
GO
ALTER TABLE [dbo].[ConsultationProfiles]  WITH CHECK ADD  CONSTRAINT [FK_ConsultationProfiles_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ConsultationProfiles] CHECK CONSTRAINT [FK_ConsultationProfiles_Users_UserId]
GO
ALTER TABLE [dbo].[Notifications]  WITH CHECK ADD  CONSTRAINT [FK_Notifications_Showrooms_ShowroomId] FOREIGN KEY([ShowroomId])
REFERENCES [dbo].[Showrooms] ([ShowroomId])
GO
ALTER TABLE [dbo].[Notifications] CHECK CONSTRAINT [FK_Notifications_Showrooms_ShowroomId]
GO
ALTER TABLE [dbo].[Notifications]  WITH CHECK ADD  CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Notifications] CHECK CONSTRAINT [FK_Notifications_Users_UserId]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK__OrderItem__CarId__5FB337D6] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK__OrderItem__CarId__5FB337D6]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK__OrderItem__Order__60A75C0F] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK__OrderItem__Order__60A75C0F]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK__Orders__CarId__619B8048] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK__Orders__CarId__619B8048]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK__Orders__Promotio__29221CFB] FOREIGN KEY([PromotionId])
REFERENCES [dbo].[Promotions] ([PromotionId])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK__Orders__Promotio__29221CFB]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK__Orders__UserId__628FA481] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK__Orders__UserId__628FA481]
GO
ALTER TABLE [dbo].[PaymentTransactions]  WITH CHECK ADD  CONSTRAINT [FK__PaymentTr__Order__6383C8BA] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[PaymentTransactions] CHECK CONSTRAINT [FK__PaymentTr__Order__6383C8BA]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD  CONSTRAINT [FK__Reviews__CarId__6477ECF3] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[Reviews] CHECK CONSTRAINT [FK__Reviews__CarId__6477ECF3]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD  CONSTRAINT [FK__Reviews__UserId__656C112C] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Reviews] CHECK CONSTRAINT [FK__Reviews__UserId__656C112C]
GO
ALTER TABLE [dbo].[SystemAuditLogs]  WITH CHECK ADD  CONSTRAINT [FK__SystemAud__UserI__1BC821DD] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[SystemAuditLogs] CHECK CONSTRAINT [FK__SystemAud__UserI__1BC821DD]
GO
ALTER TABLE [dbo].[UserActivity]  WITH CHECK ADD  CONSTRAINT [FK__UserActiv__CarId__66603565] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[UserActivity] CHECK CONSTRAINT [FK__UserActiv__CarId__66603565]
GO
ALTER TABLE [dbo].[UserActivity]  WITH CHECK ADD  CONSTRAINT [FK__UserActiv__UserI__6754599E] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[UserActivity] CHECK CONSTRAINT [FK__UserActiv__UserI__6754599E]
GO
ALTER TABLE [dbo].[UserLogins]  WITH CHECK ADD  CONSTRAINT [FK__UserLogin__UserI__503BEA1C] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[UserLogins] CHECK CONSTRAINT [FK__UserLogin__UserI__503BEA1C]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Showrooms_ShowroomId] FOREIGN KEY([ShowroomId])
REFERENCES [dbo].[Showrooms] ([ShowroomId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Showrooms_ShowroomId]
GO
USE [master]
GO
ALTER DATABASE [oto] SET  READ_WRITE 
GO
