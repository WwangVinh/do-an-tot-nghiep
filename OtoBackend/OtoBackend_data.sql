USE [master]
GO
/****** Object:  Database [oto]    Script Date: 4/28/2026 18:55:11 ******/
CREATE DATABASE [oto]
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
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[Accessories]    Script Date: 4/28/2026 18:55:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accessories](
	[AccessoryId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[ImageUrl] [nvarchar](max) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_Accessories] PRIMARY KEY CLUSTERED 
(
	[AccessoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AIRecommendations]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[ArticleCars]    Script Date: 4/28/2026 18:55:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ArticleCars](
	[ArticleId] [int] NOT NULL,
	[CarId] [int] NOT NULL,
 CONSTRAINT [PK_ArticleCars] PRIMARY KEY CLUSTERED 
(
	[ArticleId] ASC,
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Articles]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[Banners]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[Bookings]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[CarFeatures]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[CarImages]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[CarInventories]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[CarPricingVersions]    Script Date: 4/28/2026 18:55:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarPricingVersions](
	[PricingVersionId] [int] IDENTITY(1,1) NOT NULL,
	[CarId] [int] NOT NULL,
	[VersionName] [nvarchar](255) NOT NULL,
	[PriceVnd] [decimal](18, 2) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
 CONSTRAINT [PK_CarPricingVersions] PRIMARY KEY CLUSTERED 
(
	[PricingVersionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cars]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[CarSpecifications]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[CarWishlist]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[ChatMessages]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[ChatSessions]    Script Date: 4/28/2026 18:55:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChatSessions](
	[SessionId] [int] IDENTITY(1,1) NOT NULL,
	[ShowroomId] [int] NULL,
	[UserId] [int] NULL,
	[GuestToken] [nvarchar](255) NULL,
	[AssignedTo] [int] NULL,
	[Status] [nvarchar](50) NULL,
	[CreatedAt] [datetime] NULL,
	[LastMessageAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK__ChatSess__C9F49290924C9141] PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Consignments]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[ConsultationProfiles]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[Features]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[LocationTaxes]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[Notifications]    Script Date: 4/28/2026 18:55:11 ******/
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
	[RoleTarget] [nvarchar](max) NULL,
	[IsRead] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[NotificationType] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED 
(
	[NotificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 4/28/2026 18:55:11 ******/
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
	[AccessoryId] [int] NULL,
	[ItemType] [nvarchar](50) NULL,
 CONSTRAINT [PK__OrderIte__57ED06818F5EAB7E] PRIMARY KEY CLUSTERED 
(
	[OrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 4/28/2026 18:55:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
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
	[FullName] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Email] [nvarchar](255) NULL,
	[CustomerNote] [nvarchar](max) NULL,
	[SecretToken] [nvarchar](max) NULL,
	[AdminNote] [nvarchar](max) NULL,
	[LastUpdated] [datetime] NULL,
	[StaffId] [int] NULL,
 CONSTRAINT [PK__Orders__C3905BCFC3E9C951] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentTransactions]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[Promotions]    Script Date: 4/28/2026 18:55:11 ******/
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
	[CarId] [int] NULL,
	[MaxUsage] [int] NULL,
	[CurrentUsage] [int] NOT NULL,
 CONSTRAINT [PK__Promotio__52C42FCF8A6988C7] PRIMARY KEY CLUSTERED 
(
	[PromotionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reviews]    Script Date: 4/28/2026 18:55:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reviews](
	[ReviewId] [int] IDENTITY(1,1) NOT NULL,
	[CarId] [int] NULL,
	[Rating] [int] NULL,
	[Comment] [nvarchar](1000) NULL,
	[CreatedAt] [datetime] NULL,
	[FullName] [nvarchar](255) NULL,
	[Phone] [nvarchar](20) NULL,
	[OrderCode] [nvarchar](50) NULL,
	[IsApproved] [bit] NOT NULL,
 CONSTRAINT [PK__Reviews__74BC79CE36435DC2] PRIMARY KEY CLUSTERED 
(
	[ReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Showrooms]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[SystemAuditLogs]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[SystemSettings]    Script Date: 4/28/2026 18:55:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemSettings](
	[SettingKey] [varchar](50) NOT NULL,
	[SettingValue] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[SettingKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserActivity]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[UserLogins]    Script Date: 4/28/2026 18:55:11 ******/
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
/****** Object:  Table [dbo].[Users]    Script Date: 4/28/2026 18:55:11 ******/
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
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260426152359_Update', N'8.0.24')
GO
SET IDENTITY_INSERT [dbo].[Banners] ON 
GO
INSERT [dbo].[Banners] ([BannerId], [ImageUrl], [LinkUrl], [Position], [IsActive], [BannerName], [StartDate], [EndDate]) VALUES (1, N'/uploads/Banners/banner/banner-b525a7eee9f643d2af4b23712681fe9b.jpg', NULL, 1, 1, N'banner 1', NULL, NULL)
GO
INSERT [dbo].[Banners] ([BannerId], [ImageUrl], [LinkUrl], [Position], [IsActive], [BannerName], [StartDate], [EndDate]) VALUES (2, N'/uploads/Banners/banner/banner-642fafa8f6a742ef9795ac66d6264806.jpg', NULL, 0, 1, N'banner 2', NULL, NULL)
GO
INSERT [dbo].[Banners] ([BannerId], [ImageUrl], [LinkUrl], [Position], [IsActive], [BannerName], [StartDate], [EndDate]) VALUES (3, N'/uploads/Banners/banner/banner-787b876214df44568d4a32be4c8d2699.jpg', NULL, 0, 1, N'banner 3', NULL, NULL)
GO
INSERT [dbo].[Banners] ([BannerId], [ImageUrl], [LinkUrl], [Position], [IsActive], [BannerName], [StartDate], [EndDate]) VALUES (4, N'/uploads/Banners/banner/banner-77e0da6765ee4cc5a74a574612c39ca5.jpg', N'http://localhost:5173/san-pham/xe/1', 0, 0, N'banner 4', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Banners] OFF
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (1, 1)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (3, 2)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (4, 3)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (8, 3)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (6, 4)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (4, 5)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (8, 5)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (10, 6)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (2, 7)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (5, 8)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (5, 9)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (6, 10)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (2, 11)
GO
SET IDENTITY_INSERT [dbo].[CarImages] ON 
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (1, 1, N'/uploads/Cars/TEST/TEST_TEST_1/1e5b4119-5d6c-4521-8f34-093f28834fcd.jpg', 0, N'Color', N'313921f47843ca577afe977815aea637', CAST(N'2026-04-27T00:23:00.060' AS DateTime), 0, N'Màu xe', N'xanh')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (2, 1, N'/uploads/Cars/TEST/TEST_TEST_1/0cf6fd82-36af-480c-aa3b-195f7eb6d335.jpg', 0, N'Overview', N'f9439695412464f1a9eb39fc406eb87b', CAST(N'2026-04-27T00:23:00.087' AS DateTime), 0, N'1', N'1')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (3, 1, N'/uploads/Cars/TEST/TEST_TEST_1/436b34e3-eabf-4735-8fbb-a34ff238161e.png', 0, N'Exterior', N'd012fd04f53e7a1c35a75c32062aa680', CAST(N'2026-04-27T00:23:00.097' AS DateTime), 0, N'1', N'1')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (4, 1, N'/uploads/Cars/TEST/TEST_TEST_1/f589e044-d582-46c8-8a41-cd9b668c6663.jpg', 0, N'Interior', N'5fb7a0960cb48f698b16d9d4f171f211', CAST(N'2026-04-27T00:23:00.103' AS DateTime), 0, N'1', N'1')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (5, 1, N'/uploads/Cars/TEST/TEST_TEST_1/160d751f-2efc-4dcd-9fda-6e1d1f238c56.jpg', 0, N'Safety', N'ad2634bd4f6e6b204421dab6eb2332e5', CAST(N'2026-04-27T00:23:00.107' AS DateTime), 0, N'1', N'1')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (6, 1, N'/uploads/Cars/TEST/TEST_TEST_1/d0f79257-141a-41eb-94e9-b8b92ccded46.png', 0, N'Performance', N'90731d235980930c94262bf0728b75e9', CAST(N'2026-04-27T00:23:00.120' AS DateTime), 0, N'1', N'1')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (7, 1, N'/uploads/Cars/TEST/TEST_TEST_1/ac9551ff-7b7f-4464-a273-c272d4503d2d.png', 0, N'Other', N'2d263e339e7e4df5f238110cae625a7e', CAST(N'2026-04-27T00:23:00.150' AS DateTime), 0, N'Khác', N'Khác')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (8, 1, N'/uploads/Cars/TEST/TEST_TEST_1/28b690d6-1714-46a8-996f-f80aa4ce1b44.png', 0, N'Other', N'42c329b7daa355ff569ac43f13e8ff0c', CAST(N'2026-04-27T00:23:00.200' AS DateTime), 0, N'Khác', N'Khác')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (9, 2, N'/uploads/Cars/vf3-main.png', 1, N'main', NULL, CAST(N'2026-04-28T17:54:32.763' AS DateTime), 0, N'Ảnh đại diện', N'VF3 - Ảnh đại diện')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (10, 2, N'/uploads/Cars/vf3-ngoai-that-1.png', 0, N'Ngoại thất', NULL, CAST(N'2026-04-28T17:54:32.763' AS DateTime), 0, N'Ngoại thất', N'Góc nghiêng')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (11, 2, N'/uploads/Cars/vf3-noi-that-1.png', 0, N'Nội thất', NULL, CAST(N'2026-04-28T17:54:32.763' AS DateTime), 0, N'Nội thất', N'Khoang lái')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (12, 3, N'/uploads/Cars/vf5-main.png', 1, N'main', NULL, CAST(N'2026-04-28T17:54:32.763' AS DateTime), 0, N'Ảnh đại diện', N'VF5 - Ảnh đại diện')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (13, 3, N'/uploads/Cars/vf5-360-001.png', 0, N'360', NULL, CAST(N'2026-04-28T17:54:32.763' AS DateTime), 1, N'Ảnh 360', N'360-001')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (14, 3, N'/uploads/Cars/vf5-360-002.png', 0, N'360', NULL, CAST(N'2026-04-28T17:54:32.763' AS DateTime), 1, N'Ảnh 360', N'360-002')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (15, 7, N'/uploads/Cars/vios-main.png', 1, N'main', NULL, CAST(N'2026-04-28T17:54:32.763' AS DateTime), 0, N'Ảnh đại diện', N'Vios - Ảnh đại diện')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (16, 8, N'/uploads/Cars/cross-main.png', 1, N'main', NULL, CAST(N'2026-04-28T17:54:32.763' AS DateTime), 0, N'Ảnh đại diện', N'Corolla Cross - Ảnh đại diện')
GO
SET IDENTITY_INSERT [dbo].[CarImages] OFF
GO
SET IDENTITY_INSERT [dbo].[CarInventories] ON 
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (1, 1, 1, 1, N'OnDisplay', CAST(N'2026-04-27T00:22:59.9358185' AS DateTime2))
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (2, 3, 2, 8, N'Available', CAST(N'2026-04-28T17:54:32.7406036' AS DateTime2))
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (3, 3, 3, 6, N'Available', CAST(N'2026-04-28T17:54:32.7406036' AS DateTime2))
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (4, 3, 4, 3, N'OnDisplay', CAST(N'2026-04-28T17:54:32.7406036' AS DateTime2))
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (5, 3, 5, 2, N'OnDisplay', CAST(N'2026-04-28T17:54:32.7406036' AS DateTime2))
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (6, 3, 6, 1, N'Pending', CAST(N'2026-04-28T17:54:32.7406036' AS DateTime2))
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (7, 4, 7, 5, N'Available', CAST(N'2026-04-28T17:54:32.7406036' AS DateTime2))
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (8, 4, 8, 2, N'OnDisplay', CAST(N'2026-04-28T17:54:32.7406036' AS DateTime2))
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (9, 5, 9, 2, N'Available', CAST(N'2026-04-28T17:54:32.7406036' AS DateTime2))
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt]) VALUES (10, 6, 10, 4, N'Available', CAST(N'2026-04-28T17:54:32.7406036' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[CarInventories] OFF
GO
SET IDENTITY_INSERT [dbo].[CarPricingVersions] ON 
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (1, 1, N'Fead', CAST(23.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-04-27T00:23:00.013' AS DateTime), CAST(N'2026-04-27T00:23:00.013' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (2, 1, N'qèw', CAST(41.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-04-27T00:23:00.040' AS DateTime), CAST(N'2026-04-27T00:23:00.040' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[CarPricingVersions] OFF
GO
SET IDENTITY_INSERT [dbo].[Cars] ON 
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (1, N'TEST 1', N'TEST', 2026, 0, CAST(23.00 AS Decimal(18, 2)), N'xanh', N'ctdfwsvghdjfgnkjopkedfbhknfdjew
gbhịớdehfbnkjdư
ưdgfebnmtgtrựnoiewiodjepnfpneqpfopqjo[fjo[qừoopqiòhmcuioèopqươpfhnpoqưihopfhqươpfjopqưhniọqưo[pdopqưnidpjqươpdjopqựdopqươpjfiopqbnviqeniopfjqeopjdfopqượpqươpfnopqjopdkqượpmfocpqeniopnqeopjfopqk[podkop[qựdfọqượiopeqjfoqẹo[f', N'TEST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/TEST/TEST_TEST_1/6b853d80-05d3-4b34-aa5c-b52c15a9f572.png', 2, CAST(N'2026-04-27T00:22:59.853' AS DateTime), CAST(N'2026-04-27T00:23:00.047' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (2, N'VinFast VF3', N'VF3', 2024, 0, CAST(281000000.00 AS Decimal(18, 2)), N'Xanh', N'Xe điện mini đô thị, nhỏ gọn, dễ lái.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg.png', 1, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-28T17:54:32.737' AS DateTime), N'Điện', 0, NULL, NULL, N'Số tự động', N'Hatchback', NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (3, N'VinFast VF5', N'VF5', 2024, 0, CAST(497000000.00 AS Decimal(18, 2)), N'Đỏ', N'Xe điện gầm cao cỡ nhỏ, phù hợp gia đình.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg.png', 1, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-28T17:54:32.737' AS DateTime), N'Điện', 0, NULL, NULL, N'Số tự động', N'SUV', NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (4, N'VinFast VF6', N'VF6', 2025, 0, CAST(647000000.00 AS Decimal(18, 2)), N'Trắng', N'Xe điện cỡ C, tiện nghi và an toàn.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg.png', 1, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-28T17:54:32.737' AS DateTime), N'Điện', 0, NULL, NULL, N'Số tự động', N'SUV', NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (5, N'VinFast VF7', N'VF7', 2025, 0, CAST(751000000.00 AS Decimal(18, 2)), N'Đen', N'SUV điện cỡ C thiết kế hiện đại.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg.png', 1, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-28T17:54:32.737' AS DateTime), N'Điện', 0, NULL, NULL, N'Số tự động', N'SUV', NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (6, N'VinFast VF9', N'VF9', 2025, 0, CAST(1349000000.00 AS Decimal(18, 2)), N'Xám', N'SUV điện 7 chỗ cao cấp.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg.png', 1, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-28T17:54:32.737' AS DateTime), N'Điện', 0, NULL, NULL, N'Số tự động', N'SUV', NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (7, N'Toyota Vios G', N'Vios G', 2024, 0, CAST(545000000.00 AS Decimal(18, 2)), N'Trắng', N'Sedan bền bỉ, tiết kiệm, phù hợp dịch vụ.', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg.png', 1, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-28T17:54:32.737' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (8, N'Toyota Corolla Cross', N'1.8V', 2024, 0, CAST(820000000.00 AS Decimal(18, 2)), N'Đen', N'Crossover đô thị, trang bị an toàn tốt.', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg.png', 1, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-28T17:54:32.737' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (9, N'Ford Everest Titanium', N'Everest', 2024, 0, CAST(1450000000.00 AS Decimal(18, 2)), N'Trắng', N'SUV 7 chỗ mạnh mẽ, đi đường dài tốt.', N'FORD', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg.png', 1, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-28T17:54:32.737' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'SUV', NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Color], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason]) VALUES (10, N'Mitsubishi Xpander Cross', N'Xpander Cross', 2024, 0, CAST(698000000.00 AS Decimal(18, 2)), N'Cam', N'MPV gầm cao, rộng rãi, dễ sử dụng.', N'MITSUBISHI', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.jpg.png', 1, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-28T17:54:32.737' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'MPV', NULL)
GO
SET IDENTITY_INSERT [dbo].[Cars] OFF
GO
SET IDENTITY_INSERT [dbo].[CarSpecifications] ON 
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (1, 1, N'a', N'àqê', N'3req4')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (2, 1, N'a', N'ưqdf', N't4r5e3ut6')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (3, 1, N'a', N'qưed', N'y5ue6ry7')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (4, 1, N'a', N'4yw5ue3', N'3q4yw5u')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (5, 1, N'4y5', N'et4ry5', N'4wy5e')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (6, 1, N'4y5', N'ty5t', N'4y5e')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (7, 1, N'4y5', N't4ry5te6', N'4y5u')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (8, 1, N'4y5', N'4yw23ue', N'4y5e3u')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (9, 1, N't4wy5eu', N'4y5u6', N'4y53eu6')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (10, 1, N't4wy5eu', N'u4y5eu6d', N'3te4qưỷ5')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (11, 1, N't4wy5eu', N'3qt4ywue', N'qyw3')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (12, 1, N't4wy5eu', N'4wy5ed', N'4yw5')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (13, 2, N'Nội thất', N'Số chỗ ngồi', N'4')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (14, 2, N'Pin', N'Quãng đường di chuyển', N'210 km')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (15, 3, N'Nội thất', N'Số chỗ ngồi', N'5')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (16, 4, N'Nội thất', N'Số chỗ ngồi', N'5')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (17, 4, N'Động cơ', N'Loại nhiên liệu', N'Điện')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (18, 5, N'Nội thất', N'Số chỗ ngồi', N'5')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (19, 6, N'Nội thất', N'Số chỗ ngồi', N'7')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (20, 7, N'Nội thất', N'Số chỗ ngồi', N'5')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (21, 7, N'Vận hành', N'Hộp số', N'Tự động')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (22, 8, N'Nội thất', N'Số chỗ ngồi', N'5')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (23, 8, N'Động cơ', N'Loại nhiên liệu', N'Xăng')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (24, 9, N'Nội thất', N'Số chỗ ngồi', N'7')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (25, 9, N'Vận hành', N'Hệ dẫn động', N'4x2/4x4 (tùy phiên bản)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (26, 10, N'Nội thất', N'Số chỗ ngồi', N'7')
GO
SET IDENTITY_INSERT [dbo].[CarSpecifications] OFF
GO
SET IDENTITY_INSERT [dbo].[Features] ON 
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (1, N'add', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (2, N'Apple CarPlay / Android Auto', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (3, N'Cruise Control', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (4, N'Ghế da', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (5, N'Cảm biến áp suất lốp', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (6, N'Phanh tay điện tử', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (7, N'Đèn pha LED', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (8, N'Cảnh báo lệch làn', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (9, N'Giữ làn chủ động', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (10, N'6 túi khí', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (11, N'Điều hòa tự động', N'/uploads/Features/default-feature.png')
GO
SET IDENTITY_INSERT [dbo].[Features] OFF
GO
SET IDENTITY_INSERT [dbo].[Notifications] ON 
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (1, NULL, NULL, N'Tưng bừng khai trương chi nhánh mới! 🎊', N'Công ty vừa mở thêm Showroom tại Hoàn Kiếm, Hà Nội. Chúc công ty ngày càng phát triển!', N'/admin/showrooms', NULL, 0, CAST(N'2026-04-26T22:30:37.2339546' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2, NULL, NULL, N'Tưng bừng khai trương chi nhánh mới! 🎊', N'Công ty vừa mở thêm Showroom tại Hải Châu, Đà Nẵng. Chúc công ty ngày càng phát triển!', N'/admin/showrooms', NULL, 0, CAST(N'2026-04-26T22:32:14.7551253' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3, 1, NULL, N'Chào mừng bạn gia nhập! 🎉', N'Xin chào Adminn, chúc bạn sớm tìm được chiếc xe ưng ý tại Showroom của chúng tôi!', N'/', NULL, 0, CAST(N'2026-04-26T23:19:17.3555694' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (4, NULL, 1, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Lê Quang Vinh vừa được cấp tài khoản ShowroomManager tại chi nhánh chúng ta.', N'/admin/users', NULL, 0, CAST(N'2026-04-27T10:59:14.0301776' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (5, NULL, 1, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Le Quang Vinh vừa được cấp tài khoản ShowroomSales tại chi nhánh chúng ta.', N'/admin/users', NULL, 0, CAST(N'2026-04-27T11:00:03.9543184' AS DateTime2), N'System')
GO
SET IDENTITY_INSERT [dbo].[Notifications] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderItems] ON 
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (1, 3, 1, 1, CAST(23.00 AS Decimal(18, 2)), NULL, N'Car')
GO
SET IDENTITY_INSERT [dbo].[OrderItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId]) VALUES (3, 1, CAST(N'2026-04-27T16:58:51.290' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-65D6', CAST(23.00 AS Decimal(18, 2)), CAST(23.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'Nguyen Van Test', N'0901234567', N'test@gmail.com', N'Tui muốn lấy xe màu đỏ nha shop!', N'f40396f6-dff2-444c-bb1e-92e7950e1262', NULL, CAST(N'2026-04-27T16:58:51.290' AS DateTime), NULL)
GO
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[Promotions] ON 
GO
INSERT [dbo].[Promotions] ([PromotionId], [PromotionName], [DiscountAmount], [Code], [DiscountPercentage], [StartDate], [EndDate], [Description], [Status], [CarId], [MaxUsage], [CurrentUsage]) VALUES (3, NULL, NULL, N'GIAM_GIA_KHUNG', CAST(100.00 AS Decimal(5, 2)), CAST(N'2026-04-27T08:50:16.617' AS DateTime), CAST(N'2026-05-27T08:50:16.617' AS DateTime), N'Giảm 100%', N'Active', NULL, 100, 3)
GO
SET IDENTITY_INSERT [dbo].[Promotions] OFF
GO
SET IDENTITY_INSERT [dbo].[Showrooms] ON 
GO
INSERT [dbo].[Showrooms] ([ShowroomId], [Name], [Province], [District], [StreetAddress], [Hotline]) VALUES (1, N'Showroom Hoàn Kiếm - Hà Nội', N'Hà Nội', N'Hoàn Kiếm', N'15 Lê Thái Tổ, Hàng Trống', N'0243.928.XXXX')
GO
INSERT [dbo].[Showrooms] ([ShowroomId], [Name], [Province], [District], [StreetAddress], [Hotline]) VALUES (2, N'Showroom Hải Châu - Đà Nẵng', N'Đà Nẵng', N'Hải Châu', N'250 Duy Tân, Hòa Thuận Nam', N'0236.365.XXXX')
GO
INSERT [dbo].[Showrooms] ([ShowroomId], [Name], [Province], [District], [StreetAddress], [Hotline]) VALUES (3, N'VinFast - Quận 1', N'TP.HCM', N'Quận 1', N'12 Nguyễn Huệ, P.Bến Nghé', N'0333436743')
GO
INSERT [dbo].[Showrooms] ([ShowroomId], [Name], [Province], [District], [StreetAddress], [Hotline]) VALUES (4, N'Toyota - Tân Bình', N'TP.HCM', N'Tân Bình', N'68 Cộng Hòa, P.4', N'0333436743')
GO
INSERT [dbo].[Showrooms] ([ShowroomId], [Name], [Province], [District], [StreetAddress], [Hotline]) VALUES (5, N'Ford - Hà Nội', N'Hà Nội', N'Cầu Giấy', N'102 Trần Duy Hưng', N'0333436743')
GO
INSERT [dbo].[Showrooms] ([ShowroomId], [Name], [Province], [District], [StreetAddress], [Hotline]) VALUES (6, N'Mitsubishi - Đà Nẵng', N'Đà Nẵng', N'Hải Châu', N'55 Núi Thành', N'0333436743')
GO
SET IDENTITY_INSERT [dbo].[Showrooms] OFF
GO
INSERT [dbo].[SystemSettings] ([SettingKey], [SettingValue], [Description]) VALUES (N'DepositPercentage', N'10', N'Phần trăm đặt cọc mặc định khi mua xe')
GO
SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (1, N'admin', N'$2a$11$VJMt1twINyJGr7sGlOgBHuvluQ79RWP6XA5Ua3vB1qIUBXoJfMYT6', N'admin@gmail.com', N'Adminn', N'0939775683', N'Admin', NULL, CAST(N'2026-04-26T23:19:17.283' AS DateTime), N'Active', 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (2, N'vinhquanli', N'$2a$11$F5DBAFASl8G0zPe.rhXDOenyaMBE6Wqh.yvEhd5R5.S7X7901eIwi', N'wwangvinh04@gmail.com', N'Lê Quang Vinh', N'09xxxxxxxx', N'ShowroomManager', NULL, CAST(N'2026-04-27T10:59:13.907' AS DateTime), N'Active', 0, NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (3, N'vinh', N'$2a$11$ERsYLCreN8LJKVuVxYZS..reu5u4/cxU/6Pv57HhV7PS2tBskw3O6', N'wwangvinh004@gmail.com', N'Le Quang Vinh', N'090xxxxxxx', N'ShowroomSales', NULL, CAST(N'2026-04-27T11:00:03.947' AS DateTime), N'Active', 0, NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
/****** Object:  Index [IX_AIRecommendations_CarId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_AIRecommendations_CarId] ON [dbo].[AIRecommendations]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AIRecommendations_UserId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_AIRecommendations_UserId] ON [dbo].[AIRecommendations]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Articles_AuthorId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_Articles_AuthorId] ON [dbo].[Articles]
(
	[AuthorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_CarId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_CarId] ON [dbo].[Bookings]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_ShowroomId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_ShowroomId] ON [dbo].[Bookings]
(
	[ShowroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_UserId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_UserId] ON [dbo].[Bookings]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarFeatures_FeatureId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_CarFeatures_FeatureId] ON [dbo].[CarFeatures]
(
	[FeatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarImages_CarId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_CarImages_CarId] ON [dbo].[CarImages]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarInventories_CarId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_CarInventories_CarId] ON [dbo].[CarInventories]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarInventories_ShowroomId_CarId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_CarInventories_ShowroomId_CarId] ON [dbo].[CarInventories]
(
	[ShowroomId] ASC,
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarPricingVersions_CarId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_CarPricingVersions_CarId] ON [dbo].[CarPricingVersions]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarSpecifications_CarId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_CarSpecifications_CarId] ON [dbo].[CarSpecifications]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarWishlist_CarId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_CarWishlist_CarId] ON [dbo].[CarWishlist]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarWishlist_UserId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_CarWishlist_UserId] ON [dbo].[CarWishlist]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChatMessages_SessionId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_ChatMessages_SessionId] ON [dbo].[ChatMessages]
(
	[SessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChatSessions_AssignedTo]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_ChatSessions_AssignedTo] ON [dbo].[ChatSessions]
(
	[AssignedTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChatSessions_UserId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_ChatSessions_UserId] ON [dbo].[ChatSessions]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Consignments_LinkedCarId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_Consignments_LinkedCarId] ON [dbo].[Consignments]
(
	[LinkedCarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Consignments_UserId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_Consignments_UserId] ON [dbo].[Consignments]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConsultationProfiles_SessionId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_ConsultationProfiles_SessionId] ON [dbo].[ConsultationProfiles]
(
	[SessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConsultationProfiles_UserId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_ConsultationProfiles_UserId] ON [dbo].[ConsultationProfiles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Notifications_ShowroomId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_Notifications_ShowroomId] ON [dbo].[Notifications]
(
	[ShowroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Notifications_UserId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_Notifications_UserId] ON [dbo].[Notifications]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_CarId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_CarId] ON [dbo].[OrderItems]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_OrderId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderId] ON [dbo].[OrderItems]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_CarId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_Orders_CarId] ON [dbo].[Orders]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_PromotionId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_Orders_PromotionId] ON [dbo].[Orders]
(
	[PromotionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PaymentTransactions_OrderId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_PaymentTransactions_OrderId] ON [dbo].[PaymentTransactions]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Promotio__A25C5AA7A444C1CD]    Script Date: 4/28/2026 18:55:11 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Promotio__A25C5AA7A444C1CD] ON [dbo].[Promotions]
(
	[Code] ASC
)
WHERE ([Code] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Reviews_CarId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_Reviews_CarId] ON [dbo].[Reviews]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SystemAuditLogs_UserId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_SystemAuditLogs_UserId] ON [dbo].[SystemAuditLogs]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserActivity_CarId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_UserActivity_CarId] ON [dbo].[UserActivity]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserActivity_UserId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_UserActivity_UserId] ON [dbo].[UserActivity]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserLogins_UserId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_UserLogins_UserId] ON [dbo].[UserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Users_ShowroomId]    Script Date: 4/28/2026 18:55:11 ******/
CREATE NONCLUSTERED INDEX [IX_Users_ShowroomId] ON [dbo].[Users]
(
	[ShowroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__536C85E4014A9D7C]    Script Date: 4/28/2026 18:55:11 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Users__536C85E4014A9D7C] ON [dbo].[Users]
(
	[Username] ASC
)
WHERE ([Username] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D105348B415492]    Script Date: 4/28/2026 18:55:11 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Users__A9D105348B415492] ON [dbo].[Users]
(
	[Email] ASC
)
WHERE ([Email] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Accessories] ADD  DEFAULT ((1)) FOR [IsActive]
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
ALTER TABLE [dbo].[CarPricingVersions] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[CarPricingVersions] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[CarPricingVersions] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
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
ALTER TABLE [dbo].[OrderItems] ADD  DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [LastUpdated]
GO
ALTER TABLE [dbo].[PaymentTransactions] ADD  DEFAULT (getdate()) FOR [TransactionDate]
GO
ALTER TABLE [dbo].[Promotions] ADD  DEFAULT ((0)) FOR [CurrentUsage]
GO
ALTER TABLE [dbo].[Reviews] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Reviews] ADD  DEFAULT ((0)) FOR [IsApproved]
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
ALTER TABLE [dbo].[ArticleCars]  WITH CHECK ADD  CONSTRAINT [FK_ArticleCars_Articles] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[Articles] ([ArticleId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ArticleCars] CHECK CONSTRAINT [FK_ArticleCars_Articles]
GO
ALTER TABLE [dbo].[ArticleCars]  WITH CHECK ADD  CONSTRAINT [FK_ArticleCars_Cars] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ArticleCars] CHECK CONSTRAINT [FK_ArticleCars_Cars]
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
ALTER TABLE [dbo].[CarPricingVersions]  WITH CHECK ADD  CONSTRAINT [FK_CarPricingVersions_Cars] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CarPricingVersions] CHECK CONSTRAINT [FK_CarPricingVersions_Cars]
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
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderItems_Accessories] FOREIGN KEY([AccessoryId])
REFERENCES [dbo].[Accessories] ([AccessoryId])
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_OrderItems_Accessories]
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
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Staff] FOREIGN KEY([StaffId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Staff]
GO
ALTER TABLE [dbo].[PaymentTransactions]  WITH CHECK ADD  CONSTRAINT [FK__PaymentTr__Order__6383C8BA] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[PaymentTransactions] CHECK CONSTRAINT [FK__PaymentTr__Order__6383C8BA]
GO
ALTER TABLE [dbo].[Promotions]  WITH CHECK ADD  CONSTRAINT [FK_Promotions_Cars] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[Promotions] CHECK CONSTRAINT [FK_Promotions_Cars]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD  CONSTRAINT [FK__Reviews__CarId__6477ECF3] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[Reviews] CHECK CONSTRAINT [FK__Reviews__CarId__6477ECF3]
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
