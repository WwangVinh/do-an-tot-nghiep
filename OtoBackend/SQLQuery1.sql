USE [master]
GO
/****** Object:  Database [oto]    Script Date: 6/8/2026 2:38:18 ******/
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
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[Accessories]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[AIRecommendations]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[ArticleCars]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[Articles]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[Banners]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[Bookings]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[CarAccessories]    Script Date: 6/8/2026 2:38:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarAccessories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CarId] [int] NOT NULL,
	[AccessoryId] [int] NOT NULL,
 CONSTRAINT [PK_CarAccessories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarColors]    Script Date: 6/8/2026 2:38:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarColors](
	[CarColorId] [int] IDENTITY(1,1) NOT NULL,
	[CarId] [int] NOT NULL,
	[ColorName] [nvarchar](100) NOT NULL,
	[HexCode] [nvarchar](20) NULL,
	[ImageUrl] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime] NULL,
 CONSTRAINT [PK_CarColors] PRIMARY KEY CLUSTERED 
(
	[CarColorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarFeatures]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[CarImages]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[CarInventories]    Script Date: 6/8/2026 2:38:19 ******/
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
	[CarColorId] [int] NULL,
 CONSTRAINT [PK_CarInventories] PRIMARY KEY CLUSTERED 
(
	[InventoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarPricingVersions]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[Cars]    Script Date: 6/8/2026 2:38:19 ******/
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
	[CreatedByUserId] [int] NULL,
 CONSTRAINT [PK__Cars__68A0342EA480F904] PRIMARY KEY CLUSTERED 
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarSpecifications]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[CarWishlist]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[ChatMessages]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[ChatSessions]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[Consignments]    Script Date: 6/8/2026 2:38:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Consignments](
	[ConsignmentId] [int] IDENTITY(1,1) NOT NULL,
	[Brand] [nvarchar](100) NOT NULL,
	[Model] [nvarchar](100) NOT NULL,
	[Year] [int] NOT NULL,
	[Mileage] [decimal](18, 2) NOT NULL,
	[ConditionDescription] [nvarchar](1000) NULL,
	[ExpectedPrice] [decimal](18, 2) NOT NULL,
	[AgreedPrice] [decimal](18, 2) NULL,
	[CommissionRate] [decimal](5, 2) NULL,
	[Status] [nvarchar](50) NOT NULL,
	[LinkedCarId] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[GuestName] [nvarchar](255) NOT NULL,
	[GuestPhone] [nvarchar](20) NOT NULL,
	[GuestEmail] [nvarchar](255) NULL,
 CONSTRAINT [PK_Consignments] PRIMARY KEY CLUSTERED 
(
	[ConsignmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConsultationProfiles]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[ConsultRequests]    Script Date: 6/8/2026 2:38:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConsultRequests](
	[ConsultRequestId] [int] IDENTITY(1,1) NOT NULL,
	[CarId] [int] NOT NULL,
	[ShowroomId] [int] NOT NULL,
	[CustomerName] [nvarchar](100) NOT NULL,
	[Phone] [varchar](20) NOT NULL,
	[RequestType] [nvarchar](50) NOT NULL,
	[CustomerNote] [nvarchar](max) NULL,
	[MonthlyIncome] [decimal](18, 2) NULL,
	[DownPayment] [decimal](18, 2) NULL,
	[LoanTermMonths] [int] NULL,
	[Note] [nvarchar](max) NULL,
	[Status] [nvarchar](50) NOT NULL,
	[UserId] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[CarPricingVersionId] [int] NULL,
	[CarColorId] [int] NULL,
 CONSTRAINT [PK_ConsultRequests] PRIMARY KEY CLUSTERED 
(
	[ConsultRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Features]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[LocationTaxes]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[Notifications]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[OrderItems]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[Orders]    Script Date: 6/8/2026 2:38:19 ******/
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
	[ShowroomId] [int] NULL,
 CONSTRAINT [PK__Orders__C3905BCFC3E9C951] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentTransactions]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[Promotions]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[Reviews]    Script Date: 6/8/2026 2:38:19 ******/
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
	[UserId] [int] NULL,
 CONSTRAINT [PK__Reviews__74BC79CE36435DC2] PRIMARY KEY CLUSTERED 
(
	[ReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Showrooms]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[SystemAuditLogs]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[SystemSettings]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[UserActivity]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[UserLogins]    Script Date: 6/8/2026 2:38:19 ******/
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
/****** Object:  Table [dbo].[Users]    Script Date: 6/8/2026 2:38:19 ******/
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
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260430084541_SyncConsignmentGuest', N'8.0.24')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260502132848_Update_Car_Model', N'8.0.24')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260502192239_Update_Car_Inventory', N'8.0.24')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260502202838_Update_Car_Accessory', N'8.0.24')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260503144717_AddUserIdToReview', N'8.0.24')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260504005109_Order_Update_ShowroomId', N'8.0.24')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260510200606_AddCarColorTable_RemoveOldColorColumn', N'8.0.24')
GO
INSERT [dbo].[ArticleCars] ([ArticleId], [CarId]) VALUES (3, 11)
GO
SET IDENTITY_INSERT [dbo].[Articles] ON 
GO
INSERT [dbo].[Articles] ([ArticleId], [Title], [Thumbnail], [Content], [AuthorId], [CreatedAt], [IsPublished]) VALUES (3, N'VinFast VF9 Plus: Trải Nghiệm Tiện Nghi Đẳng Cấp Trên Mẫu SUV Điện Cao Cấp Nhất', N'https://static-cms-prod.vinfastauto.ph/statics/2024-07/VinFast_VF%209_VinFast%20Blue_A7302671%20copy.webp', N'<p data-path-to-node="5">Nếu phiên bản Eco tập trung vào sự tối ưu, thì <b data-path-to-node="5" data-index-in-node="47">VF9 Plus</b> chính là lời khẳng định về sự xa hoa và công nghệ đỉnh cao của VinFast. Đây là lựa chọn hoàn hảo cho những khách hàng tìm kiếm sự hưởng thụ thực thụ trên mọi hành trình.</p><h3 data-path-to-node="6">1. Sự khác biệt từ hệ thống chiếu sáng và mâm&nbsp;</h3><h3 data-path-to-node="6">VF9 Plus được trang bị hệ thống đèn <b data-path-to-node="7" data-index-in-node="36">Matrix LED</b> thông minh, có khả năng tự động thích ứng, giúp tối ưu tầm nhìn mà không gây chói mắt cho xe đối diện. Bộ mâm 21 inch (hoặc tùy chọn 22 inch) với thiết kế đa chấu tinh xảo giúp ngoại hình xe trở nên bề thế và sang trọng hơn hẳn.</h3><h3 data-path-to-node="8">2. Nội thất nâng tầm thượng lưu</h3><p data-path-to-node="9">Điểm ăn tiền nhất trên bản Plus chính là khoang cabin:</p><ul data-path-to-node="10"><li><p data-path-to-node="10,0,0"><b data-path-to-node="10,0,0" data-index-in-node="0">Chất liệu da thật:</b> Toàn bộ ghế ngồi được bọc da cao cấp, mang lại cảm giác mềm mại và sang trọng.</p></li><li><p data-path-to-node="10,1,0"><b data-path-to-node="10,1,0" data-index-in-node="0">Hàng ghế cơ trưởng (Captain Seat):</b> Cấu hình 6 chỗ ngồi với hàng ghế thứ 2 tách biệt, tích hợp bệ tỳ tay riêng và bảng điều khiển cảm ứng.</p></li><li><p data-path-to-node="10,2,0"><b data-path-to-node="10,2,0" data-index-in-node="0">Tiện nghi tối đa:</b> Cả hai hàng ghế đầu đều có tính năng <b data-path-to-node="10,2,0" data-index-in-node="55">massage, sưởi ấm và thông gió</b> – một trang bị hiếm thấy trong phân khúc giá này.</p></li></ul><h3 data-path-to-node="11">3. Công nghệ và Trải nghiệm lái</h3><ul data-path-to-node="12"><li><p data-path-to-node="12,0,0"><b data-path-to-node="12,0,0" data-index-in-node="0">Cửa sổ trời toàn cảnh trần kính:</b> Loại bỏ hoàn toàn cảm giác bí bách, kết nối không gian trong xe với thiên nhiên.</p></li><li><p data-path-to-node="12,1,0"><b data-path-to-node="12,1,0" data-index-in-node="0">Màn hình phụ cho hàng ghế sau:</b> Giúp hành khách phía sau dễ dàng điều chỉnh điều hòa và các tính năng giải trí độc lập.</p></li><li><p data-path-to-node="12,2,0"><b data-path-to-node="12,2,0" data-index-in-node="0">Hệ thống âm thanh:</b> 13 loa cao cấp mang đến trải nghiệm âm thanh sống động như trong một rạp hát di động.</p><img src="https://static-cms-prod.vinfastauto.ph/statics/2024-07/VinFast_VF%209_VinFast%20Blue_A7302671%20copy.webp"></li></ul><p></p>', 1, CAST(N'2026-05-01T16:12:48.770' AS DateTime), 1)
GO
SET IDENTITY_INSERT [dbo].[Articles] OFF
GO
SET IDENTITY_INSERT [dbo].[Banners] ON 
GO
INSERT [dbo].[Banners] ([BannerId], [ImageUrl], [LinkUrl], [Position], [IsActive], [BannerName], [StartDate], [EndDate]) VALUES (1, N'/uploads/Banners/banner/banner-b525a7eee9f643d2af4b23712681fe9b.jpg', NULL, 1, 1, N'banner 1', NULL, NULL)
GO
INSERT [dbo].[Banners] ([BannerId], [ImageUrl], [LinkUrl], [Position], [IsActive], [BannerName], [StartDate], [EndDate]) VALUES (2, N'/uploads/Banners/banner/banner-642fafa8f6a742ef9795ac66d6264806.jpg', NULL, 0, 1, N'banner 2', NULL, NULL)
GO
INSERT [dbo].[Banners] ([BannerId], [ImageUrl], [LinkUrl], [Position], [IsActive], [BannerName], [StartDate], [EndDate]) VALUES (3, N'/uploads/Banners/banner/banner-787b876214df44568d4a32be4c8d2699.jpg', NULL, 0, 1, N'banner 3', NULL, NULL)
GO
INSERT [dbo].[Banners] ([BannerId], [ImageUrl], [LinkUrl], [Position], [IsActive], [BannerName], [StartDate], [EndDate]) VALUES (4, N'/uploads/Banners/banner/banner-77e0da6765ee4cc5a74a574612c39ca5.jpg', N'http://localhost:5173/san-pham/xe/1', 0, 1, N'banner 4', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Banners] OFF
GO
SET IDENTITY_INSERT [dbo].[Bookings] ON 
GO
INSERT [dbo].[Bookings] ([BookingId], [CarId], [CustomerName], [Phone], [BookingDate], [BookingTime], [Note], [CreatedAt], [UserId], [Status], [ShowroomId], [UpdatedAt]) VALUES (6, 11, N'WwangVinh04', N'0965346160', CAST(N'2026-04-30' AS Date), N'20:07', N'Đăng ký lái thử phiên bản [VF 9 Plus (Thuê pin)] tại [Showroom Hoàn Kiếm - Hà Nội]
[29/04/2026 15:24 - Admin]: Kết quả tư vấn: đã tư vấn xong
[29/04/2026 15:25 - Admin]: Ghi chú gửi kỹ thuật: nhân viên gửi yêu cầu qua bên kĩ thuật chuẩn bị xe chờ ngày lái thử
[29/04/2026 15:25 - Admin]: Kỹ thuật DUYỆT xe: kĩ thuật đã kiểm tra xong và đảm bảo có xe trong giờ và ngày hôm đó
[29/04/2026 15:25 - Admin]: Đã xác nhận lịch lái thử với khách.
[29/04/2026 15:26 - Admin]: Kết quả lái thử: khách đã ưng ý', CAST(N'2026-04-29T15:07:27.827' AS DateTime), NULL, N'Completed', 1, CAST(N'2026-04-29T15:26:16.5841009' AS DateTime2))
GO
INSERT [dbo].[Bookings] ([BookingId], [CarId], [CustomerName], [Phone], [BookingDate], [BookingTime], [Note], [CreatedAt], [UserId], [Status], [ShowroomId], [UpdatedAt]) VALUES (7, 11, N'WwangVinh04', N'0965346160', CAST(N'2026-04-30' AS Date), N'00:12', N'Đăng ký lái thử phiên bản [VF 9 Plus (Thuê pin)] tại [Showroom Hoàn Kiếm - Hà Nội]', CAST(N'2026-04-29T20:12:17.440' AS DateTime), NULL, N'Pending', 1, NULL)
GO
INSERT [dbo].[Bookings] ([BookingId], [CarId], [CustomerName], [Phone], [BookingDate], [BookingTime], [Note], [CreatedAt], [UserId], [Status], [ShowroomId], [UpdatedAt]) VALUES (8, 11, N'Lê Vinh', N'0965346160', CAST(N'2026-05-14' AS Date), N'01:33', N'Đăng ký lái thử phiên bản [VF 9 Plus (Kèm pin)] tại [Showroom Hoàn Kiếm - Hà Nội]', CAST(N'2026-04-30T23:33:37.080' AS DateTime), NULL, N'Pending', 1, NULL)
GO
INSERT [dbo].[Bookings] ([BookingId], [CarId], [CustomerName], [Phone], [BookingDate], [BookingTime], [Note], [CreatedAt], [UserId], [Status], [ShowroomId], [UpdatedAt]) VALUES (9, 11, N'WwangVinh04', N'03814846150', CAST(N'2026-05-01' AS Date), N'23:30', N'Tư vấn báo giá xe VinFast VF 9 Plus - Trả góp', CAST(N'2026-05-01T23:30:52.520' AS DateTime), NULL, N'Pending', 1, NULL)
GO
INSERT [dbo].[Bookings] ([BookingId], [CarId], [CustomerName], [Phone], [BookingDate], [BookingTime], [Note], [CreatedAt], [UserId], [Status], [ShowroomId], [UpdatedAt]) VALUES (10, 11, N'WwangVinh04', N'03814846150', CAST(N'2026-05-01' AS Date), N'23:39', N'Tư vấn báo giá xe VinFast VF 9 Plus - Trả góp - Showroom Hải Châu - Đà Nẵng
[06/05/2026 04:10 - Admin]: Kết quả tư vấn: .', CAST(N'2026-05-01T23:39:25.443' AS DateTime), NULL, N'PendingTechCheck', 2, CAST(N'2026-05-06T04:10:22.5387831' AS DateTime2))
GO
INSERT [dbo].[Bookings] ([BookingId], [CarId], [CustomerName], [Phone], [BookingDate], [BookingTime], [Note], [CreatedAt], [UserId], [Status], [ShowroomId], [UpdatedAt]) VALUES (1009, 6, N'èt', N'03814846150', CAST(N'2026-05-07' AS Date), N'21:50', N'Tư vấn báo giá xe VinFast VF9 - Trả góp - VinFast - Quận 1
[07/05/2026 21:50 - Admin]: Kết quả tư vấn: adsf
[07/05/2026 21:51 - Admin]: Kỹ thuật DUYỆT xe: gr
[07/05/2026 21:51 - Admin]: Đã xác nhận lịch lái thử với khách.
[07/05/2026 21:51 - Admin]: Kết quả lái thử: ưqfsewgr', CAST(N'2026-05-07T21:50:38.063' AS DateTime), NULL, N'Completed', 3, CAST(N'2026-05-07T21:51:09.6123416' AS DateTime2))
GO
INSERT [dbo].[Bookings] ([BookingId], [CarId], [CustomerName], [Phone], [BookingDate], [BookingTime], [Note], [CreatedAt], [UserId], [Status], [ShowroomId], [UpdatedAt]) VALUES (1010, 11, N'WwangVinh04', N'0965346160', CAST(N'2026-05-08' AS Date), N'08:30', N'Tư vấn mua trả góp - VinFast VF 9 Plus - Showroom Hoàn Kiếm - Hà Nội', CAST(N'2026-05-08T08:30:17.563' AS DateTime), NULL, N'Pending', 1, NULL)
GO
INSERT [dbo].[Bookings] ([BookingId], [CarId], [CustomerName], [Phone], [BookingDate], [BookingTime], [Note], [CreatedAt], [UserId], [Status], [ShowroomId], [UpdatedAt]) VALUES (1011, 11, N'WwangVinh04', N'0965346160', CAST(N'2026-05-08' AS Date), N'19:51', N'Tư vấn mua trả góp - VinFast VF 9 Plus - Showroom Hoàn Kiếm - Hà Nội', CAST(N'2026-05-08T19:51:53.233' AS DateTime), NULL, N'Pending', 1, NULL)
GO
INSERT [dbo].[Bookings] ([BookingId], [CarId], [CustomerName], [Phone], [BookingDate], [BookingTime], [Note], [CreatedAt], [UserId], [Status], [ShowroomId], [UpdatedAt]) VALUES (2010, 11, N'WwangVinh04', N'0965346160', CAST(N'2026-05-11' AS Date), N'23:00', N'Tư vấn mua trả góp - VinFast VF 9 Plus - Showroom Hoàn Kiếm - Hà Nội
[12/05/2026 00:50 - Admin]: Kết quả tư vấn: 0
[12/05/2026 03:54 - Admin]: Kỹ thuật DUYỆT xe: d
[12/05/2026 03:54 - Admin]: Đã xác nhận lịch lái thử với khách.
[12/05/2026 03:55 - Admin]: Kết quả lái thử: đã lái thử và thấy ok', CAST(N'2026-05-11T23:00:10.733' AS DateTime), NULL, N'Completed', 1, CAST(N'2026-05-12T03:55:08.3946663' AS DateTime2))
GO
INSERT [dbo].[Bookings] ([BookingId], [CarId], [CustomerName], [Phone], [BookingDate], [BookingTime], [Note], [CreatedAt], [UserId], [Status], [ShowroomId], [UpdatedAt]) VALUES (2011, 11, N'WwangVinh04', N'3814846150', CAST(N'2026-05-28' AS Date), N'08:58', N'Đăng ký lái thử phiên bản [VF 9 Plus (Thuê pin)] tại [Showroom Hoàn Kiếm - Hà Nội]', CAST(N'2026-05-12T03:53:32.960' AS DateTime), NULL, N'Pending', 1, NULL)
GO
SET IDENTITY_INSERT [dbo].[Bookings] OFF
GO
SET IDENTITY_INSERT [dbo].[CarColors] ON 
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (5, 5, N'Đen', NULL, NULL, 1, CAST(N'2026-05-10T20:08:09.360' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (6, 6, N'Xám', NULL, NULL, 1, CAST(N'2026-05-10T20:08:09.360' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (7, 7, N'Trắng', NULL, NULL, 1, CAST(N'2026-05-10T20:08:09.360' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (8, 8, N'Đen', NULL, NULL, 1, CAST(N'2026-05-10T20:08:09.360' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (9, 9, N'Trắng', NULL, NULL, 1, CAST(N'2026-05-10T20:08:09.360' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (10, 10, N'Cam', NULL, NULL, 1, CAST(N'2026-05-10T20:08:09.360' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (11, 11, N'xanh, xanh rêu', NULL, NULL, 1, CAST(N'2026-05-10T20:08:09.360' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (13, 14, N'Đen ánh độc tôn', N'#0D0D0D', NULL, 1, CAST(N'2026-05-11T18:24:28.083' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (14, 14, N'Trắng ngọc quý phái', N'#F4F6F7', NULL, 1, CAST(N'2026-05-11T18:24:28.083' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (15, 14, N'Xám phong cách', N'#595F62', NULL, 1, CAST(N'2026-05-11T18:24:28.083' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (16, 14, N'Xanh lịch lãm', N'#1C2D3D', NULL, 1, CAST(N'2026-05-11T18:24:28.083' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (17, 14, N'Đỏ cá tính', N'#9E0B0E', NULL, 1, CAST(N'2026-05-11T18:24:28.083' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (28, 16, N'Trắng Chìm Đắm', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:07:11.217' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (29, 16, N'Đen Sang Trọng', N'#000000', NULL, 1, CAST(N'2026-05-11T19:07:11.217' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (30, 2, N'Xanh', N'#000000', NULL, 1, CAST(N'2026-05-11T19:08:37.737' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (31, 3, N'Đỏ', N'#000000', NULL, 1, CAST(N'2026-05-11T19:08:49.557' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (32, 4, N'Trắng', N'#000000', NULL, 1, CAST(N'2026-05-11T19:09:01.157' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (33, 15, N'Trắng Chìm Đắm', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:09:34.353' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (34, 15, N'Đen Sang Trọng', N'#0B0C10', NULL, 1, CAST(N'2026-05-11T19:09:34.353' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (35, 15, N'Xám Thời Thượng', N'#5C5D5D', NULL, 1, CAST(N'2026-05-11T19:09:34.353' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (36, 15, N'Đỏ Quyền Lực', N'#A6192E', NULL, 1, CAST(N'2026-05-11T19:09:34.353' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (37, 15, N'Xanh Biển Sâu', N'#1A365D', NULL, 1, CAST(N'2026-05-11T19:09:34.353' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (101, 101, N'Trắng Ngọc Trai', N'#F4F6F7', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (102, 101, N'Đen Ánh', N'#000000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (103, 101, N'Đỏ', N'#8B0000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (104, 102, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (105, 102, N'Xám', N'#595F62', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (106, 102, N'Đỏ Mận', N'#65000B', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (107, 102, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (108, 103, N'Bạc', N'#C0C0C0', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (109, 103, N'Đồng', N'#B87333', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (110, 103, N'Đen', N'#0D0D0D', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (111, 103, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (112, 104, N'Bạc', N'#E0E0E0', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (113, 104, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (114, 104, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (115, 104, N'Đỏ', N'#FF0000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (116, 104, N'Vàng Cát', N'#C2B280', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (117, 105, N'Đỏ', N'#B22222', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (118, 105, N'Đen', N'#111111', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (119, 105, N'Trắng', N'#FAFAFA', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (120, 105, N'Bạc', N'#D3D3D3', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (121, 106, N'Xanh Đại Dương', N'#0047AB', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (122, 106, N'Đỏ Thẫm', N'#800000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (123, 106, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (124, 106, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (125, 107, N'Vàng Khoe', N'#FFD700', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (126, 107, N'Cam', N'#FF8C00', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (127, 107, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (128, 107, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (129, 108, N'Xanh Sẫm', N'#00008B', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (130, 108, N'Đỏ', N'#DC143C', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (131, 108, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (132, 108, N'Xám', N'#808080', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (133, 109, N'Xanh Rêu', N'#2F4F4F', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (134, 109, N'Xám Không Gian', N'#696969', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (135, 109, N'Trắng', N'#F5F5F5', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (136, 109, N'Đen', N'#0A0A0A', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (137, 110, N'Nâu', N'#8B4513', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (138, 110, N'Bạc', N'#C0C0C0', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (139, 110, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (140, 110, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (141, 111, N'Vàng Đậm', N'#FFB300', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (142, 111, N'Đỏ', N'#FF0000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (143, 111, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (144, 111, N'Xám Ximăng', N'#A9A9A9', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (145, 112, N'Đỏ Cam', N'#FF4500', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (146, 112, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (147, 112, N'Bạc', N'#E8E8E8', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (148, 112, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (149, 113, N'Xanh Lam', N'#1E90FF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (150, 113, N'Đỏ Mận', N'#8B0000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (151, 113, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (152, 113, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (153, 114, N'Đỏ Pha Lê', N'#9E0108', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (154, 114, N'Xám Machine', N'#4A4A4A', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (155, 114, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (156, 114, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (157, 115, N'Đỏ Soul Red', N'#8B0000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (158, 115, N'Trắng Snowflake', N'#FDFDFD', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (159, 115, N'Đen', N'#0A0A0A', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (160, 115, N'Xám', N'#808080', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (161, 116, N'Đỏ Cherry', N'#D2042D', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (162, 116, N'Titan', N'#878681', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (163, 116, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (164, 116, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (165, 117, N'Xám Ghi', N'#708090', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (166, 117, N'Trắng Ngọc', N'#F8F8FF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (167, 117, N'Đỏ', N'#FF0000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (168, 117, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (169, 118, N'Xanh Rêu', N'#556B2F', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (170, 118, N'Xanh Dương', N'#4682B4', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (171, 118, N'Bạc', N'#C0C0C0', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (172, 118, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (173, 119, N'Xanh Cổ Vịt', N'#008080', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (174, 119, N'Đỏ', N'#B22222', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (175, 119, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (176, 119, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (177, 120, N'Khaki (Xanh rêu)', N'#BDB76B', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (178, 120, N'Cam', N'#FF8C00', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (179, 120, N'Đỏ', N'#DC143C', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (180, 120, N'Xám', N'#696969', NULL, 1, CAST(N'2026-05-11T19:25:43.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (359, 150, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (360, 150, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (361, 151, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (362, 151, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (363, 152, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (364, 152, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (365, 153, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (366, 153, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (367, 154, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (368, 154, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (369, 155, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (370, 155, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (371, 156, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (372, 156, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (373, 157, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (374, 157, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (375, 158, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (376, 158, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (377, 159, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (378, 159, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (379, 160, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (380, 160, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (381, 161, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (382, 161, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (383, 162, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (384, 162, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (385, 163, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (386, 163, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (387, 164, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (388, 164, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (389, 165, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (390, 165, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (391, 166, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (392, 166, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (393, 167, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (394, 167, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (395, 168, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (396, 168, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (397, 169, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (398, 169, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (399, 170, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (400, 170, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (401, 171, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (402, 171, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (403, 172, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (404, 172, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (405, 173, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (406, 173, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (407, 174, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (408, 174, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (409, 175, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (410, 175, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (411, 176, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (412, 176, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (413, 177, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (414, 177, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (415, 178, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (416, 178, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (417, 179, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (418, 179, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (419, 180, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (420, 180, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (421, 181, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (422, 181, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (423, 182, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (424, 182, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (425, 183, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (426, 183, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (427, 184, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (428, 184, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (429, 185, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (430, 185, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (431, 186, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (432, 186, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (433, 187, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (434, 187, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (435, 188, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (436, 188, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (437, 189, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (438, 189, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (439, 190, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (440, 190, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (441, 191, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (442, 191, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (443, 192, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (444, 192, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (445, 193, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (446, 193, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (447, 194, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (448, 194, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (449, 195, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (450, 195, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (451, 196, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (452, 196, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (453, 197, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (454, 197, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (455, 198, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (456, 198, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (457, 199, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (458, 199, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (459, 200, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (460, 200, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T19:32:26.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (466, 121, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T23:55:18.140' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (467, 121, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T23:55:18.140' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (468, 122, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T23:57:32.897' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (469, 122, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T23:57:32.897' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (470, 123, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T23:58:08.423' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (471, 123, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T23:58:08.423' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (472, 124, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T23:59:10.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (473, 124, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T23:59:10.807' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (474, 125, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-11T23:59:46.673' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (475, 125, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-11T23:59:46.673' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (478, 126, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:00:44.450' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (479, 126, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:00:44.450' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (480, 127, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:01:08.143' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (481, 127, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:01:08.143' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (482, 128, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:01:53.847' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (483, 128, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:01:53.847' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (484, 129, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:02:24.300' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (485, 129, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:02:24.300' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (486, 130, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:02:56.170' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (487, 130, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:02:56.170' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (488, 131, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:03:57.030' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (489, 131, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:03:57.030' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (490, 132, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:04:28.467' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (491, 132, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:04:28.467' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (492, 133, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:05:05.027' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (493, 133, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:05:05.027' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (494, 134, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:05:43.093' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (495, 134, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:05:43.093' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (496, 135, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:06:12.127' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (497, 135, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:06:12.127' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (502, 137, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:07:34.303' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (503, 137, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:07:34.303' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (504, 136, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:07:40.890' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (505, 136, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:07:40.890' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (506, 138, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:09:19.443' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (507, 138, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:09:19.443' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (508, 139, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:09:46.460' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (509, 139, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:09:46.463' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (510, 140, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:10:15.490' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (511, 140, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:10:15.490' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (512, 141, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:11:37.247' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (513, 141, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:11:37.247' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (514, 142, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:12:01.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (515, 142, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:12:01.757' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (516, 143, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:12:30.007' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (517, 143, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:12:30.007' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (518, 144, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:13:09.833' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (519, 144, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:13:09.833' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (520, 145, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:13:35.637' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (521, 145, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:13:35.637' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (522, 146, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:14:08.760' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (523, 146, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:14:08.760' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (524, 147, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:14:41.377' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (525, 147, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:14:41.377' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (526, 148, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:15:15.307' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (527, 148, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:15:15.307' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (528, 149, N'Trắng', N'#FFFFFF', NULL, 1, CAST(N'2026-05-12T00:15:43.083' AS DateTime))
GO
INSERT [dbo].[CarColors] ([CarColorId], [CarId], [ColorName], [HexCode], [ImageUrl], [IsActive], [CreatedAt]) VALUES (529, 149, N'Đen', N'#000000', NULL, 1, CAST(N'2026-05-12T00:15:43.083' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[CarColors] OFF
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (2, 7)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (2, 11)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (3, 2)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (4, 3)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (4, 5)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (5, 8)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (5, 9)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (6, 4)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (6, 10)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (8, 3)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (8, 5)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (10, 6)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (11, 14)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (11, 15)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (11, 16)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (11, 17)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (11, 18)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (11, 19)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 19)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 20)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 21)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 22)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 23)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 24)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 25)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 26)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 27)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 28)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 29)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 30)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 31)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 32)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 33)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 34)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 35)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 36)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 37)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 38)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 39)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 40)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 41)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 42)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (14, 43)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (15, 14)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (15, 15)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (15, 44)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (15, 45)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (15, 46)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (15, 47)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (15, 48)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (15, 49)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (15, 50)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (15, 51)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (15, 52)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (15, 53)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (16, 15)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (16, 19)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (16, 48)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (16, 52)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (16, 54)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (16, 55)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (16, 56)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (16, 57)
GO
INSERT [dbo].[CarFeatures] ([CarId], [FeatureId]) VALUES (16, 58)
GO
SET IDENTITY_INSERT [dbo].[CarImages] ON 
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
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (17, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/dae6df36-1b1f-4347-8907-befe1aa35f96.png', 0, N'Color', N'd012fd04f53e7a1c35a75c32062aa680', CAST(N'2026-04-29T15:04:39.623' AS DateTime), 0, N'Màu xe', N'xanh')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (18, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/9b115b82-3065-49fe-b3be-07b1ff68a7fc.jpg', 0, N'Color', N'313921f47843ca577afe977815aea637', CAST(N'2026-04-29T15:04:39.640' AS DateTime), 0, N'Màu xe', N'xanh rêu')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (19, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/fee6244f-26fa-4213-b56f-80997131b254.jpg', 0, N'Overview', N'1f5cc6e58ec918235231f0e55056647c', CAST(N'2026-04-29T15:04:39.643' AS DateTime), 0, N'Sở hữu kiểu dáng SUV full-size bề thế, VinFast VF 9 Plus gây ấn tượng mạnh với thiết kế liền mạch và tối ưu khí động học. Từ góc nhìn này, xe nổi bật với trần kính toàn cảnh (Panoramic Sunroof) vuốt dài ra phía sau, kết hợp cùng các đường gân dập nổi mạnh mẽ dọc thân xe và tay nắm cửa dạng ẩn. Tất cả hòa quyện tạo nên một diện mạo vừa sang trọng, đẳng cấp vừa đậm chất tương lai.', N'Tổng quan góc chéo từ trên cao')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (20, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/c17f24f0-7d34-4550-abc8-63cc27100cea.jpg', 0, N'Overview', N'1f86762833bb5c53aae0ea041ded51f6', CAST(N'2026-04-29T15:04:39.650' AS DateTime), 0, N'Nhìn từ trực diện, mặt ca-lăng của VF 9 Plus thể hiện sự uy nghi và bề thế của dòng xe SUV điện đầu bảng. Điểm nhấn đặc trưng là dải đèn định vị ban ngày LED vuốt nhọn hình cánh chim, ôm trọn logo chữ ''V'' tỏa sáng ở trung tâm. Phần cản trước được thiết kế mở rộng, nam tính, kết hợp cùng cụm đèn pha Matrix LED sắc sảo mang đến ánh nhìn đầy quyền lực và cuốn hút.', N'Tổng quan trực diện phía trước hoặc Thiết kế đầu xe.')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (21, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/b9ec52a0-301f-4ee0-9bb8-a85ee3c72354.jpg', 0, N'Exterior', N'45b3354cf9557e7878ffe377be40b73a', CAST(N'2026-04-29T15:04:39.653' AS DateTime), 0, N'Được trang bị bộ la-zăng hợp kim nguyên khối kích thước lên tới 21 inch (tùy chọn 22 inch) với thiết kế đa chấu phối hai tông màu thể thao. Cấu trúc mâm xe được tinh chỉnh tối ưu hóa tính khí động học, không chỉ tôn lên vẻ vững chãi, bề thế mà còn giúp xe di chuyển êm ái, đầm chắc.', N'Chi tiết Mâm xe (La-zăng')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (22, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/6cc87d5e-4f9c-43e4-88fd-8649ecd5e065.jpg', 0, N'Exterior', N'95ce34f50f8676e915323cd6402ff0f5', CAST(N'2026-04-29T15:04:39.657' AS DateTime), 0, N'Phần đuôi xe vuông vức, nam tính tạo không gian rộng rãi cho hàng ghế thứ 3. Điểm nhấn là dải đèn hậu LED rực rỡ vắt ngang đuôi xe, ôm trọn logo chữ V, tạo nên sự liền mạch với thiết kế đầu xe và mang lại độ nhận diện thương hiệu tuyệt đối dù là ban ngày hay đêm tối.', N'Chi tiết Đuôi xe & Cụm đèn hậu')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (23, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/fe933cd4-5ff2-4f0c-8459-ec98f5f1a9df.jpg', 0, N'Exterior', N'deadb1b66b95e384e6cf3b04434d3b0b', CAST(N'2026-04-29T15:04:39.663' AS DateTime), 0, N'Hệ thống chiếu sáng công nghệ Matrix LED cao cấp tích hợp khả năng tự động bật/tắt và điều chỉnh góc chiếu. Đèn có khả năng tự động thích ứng thông minh, chống chói cho xe ngược chiều, đảm bảo tầm nhìn hoàn hảo và an toàn tối đa cho người lái.', N'Chi tiết Cụm đèn chiếu sáng (Nếu muốn nhấn mạnh thêm công nghệ)')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (24, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/f98bfad2-7af9-4a89-93a8-3824442ea073.jpg', 0, N'Interior', N'573bc9b4179760f6fcc23e9d2a49070a', CAST(N'2026-04-29T15:04:39.670' AS DateTime), 0, N'6 chỗ ngồi với ghế cơ trưởng thiết kế thời thượng và trang bị công nghệ vượt trội như: Có bệ tì tay và hộp để đồ tích hợp sạc không dây tiện lợi, khả năng chỉnh điện 8 hướng, hỗ trợ tính năng massage, thông gió và sưởi. Lựa chọn này có chi phí phụ thêm là 32,23 triệu đồng (đã bao gồm VAT). Cho đến thời điểm hiện nay, đây là mẫu xe hiếm hoi trong phân khúc SUV đô thị cỡ lớn tại Việt Nam sở hữu thiết kế này, hứa hẹn sẽ mang đến những trải nghiệm đẳng cấp, mới mẻ cho khách hàng.', N'tùy chọn chỗ ngồi')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (25, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/c4ddc682-fd53-451a-91f6-b7572d273a08.png', 0, N'Interior', N'1b30718cbdb31de9c40572b5c09fbfa1', CAST(N'2026-04-29T15:04:39.673' AS DateTime), 0, N'Nội thất VF 9 Plus được thiết kế theo phong cách tối giản nhưng cực kỳ sang trọng với chất lượng hoàn thiện cao cấp. Bảng táp-lô lược bỏ các nút bấm vật lý truyền thống, thay vào đó là sự kết hợp tinh tế giữa da cao cấp, các chi tiết ốp trang trí hiện đại và hệ thống đèn viền nội thất (Ambient Light) đa màu sắc, tạo nên không gian đậm chất công nghệ.', N'Khoang lái hiện đại và tối giản')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (26, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/c2a84e87-15aa-44f6-912b-583403eb8d41.png', 0, N'Interior', N'547a6eb90d55a7da162140d3b149f495', CAST(N'2026-04-29T15:04:39.677' AS DateTime), 0, N'Mọi tính năng giải trí và điều khiển xe được tích hợp trong màn hình cảm ứng trung tâm kích thước lớn 15.6 inch với độ phân giải sắc nét. Hệ thống sử dụng chip xử lý mạnh mẽ, hỗ trợ trợ lý ảo thông minh, kết nối đa phương tiện và cho phép cá nhân hóa tối đa trải nghiệm người dùng ngay trên đầu ngón tay.', N'Trung tâm điều khiển thông minh 15.6 inch')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (27, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/505e78f5-1385-4d33-9fa9-bee9d31385fa.jpg', 0, N'Safety', N'65768abe8e479582cd6766b8741383c9', CAST(N'2026-04-29T15:04:39.680' AS DateTime), 0, N'VinFast VF 9 Plus được trang bị hệ thống hỗ trợ lái xe nâng cao (ADAS) cấp độ 2 với hàng loạt tính năng thông minh như: hỗ trợ di chuyển khi ùn tắc, hỗ trợ lái trên cao tốc, cảnh báo chệch làn, hỗ trợ giữ làn và tự động chuyển làn. Tất cả mang đến sự an tâm tuyệt đối trên mọi hành trình.', N'Hệ thống trợ lái nâng cao ADAS')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (28, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/ae83e0c7-e08b-4582-9483-468beed40b66.jpg', 0, N'Performance', N'e59c7a5ee5f45926db3400e3601aa065', CAST(N'2026-04-29T15:04:39.683' AS DateTime), 0, N'Với khối pin dung lượng lớn, VF 9 Plus cho phạm vi hoạt động lên tới hơn 400 km sau một lần sạc đầy (theo chuẩn WLTP). Xe hỗ trợ chuẩn sạc siêu nhanh, chỉ mất khoảng 26 phút để sạc từ 10% lên 70%, tối ưu thời gian cho những chuyến đi dài.', N'Pin dung lượng lớn & Sạc nhanh')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (29, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/3774e9c9-c6e9-4e08-9ad0-120336585947.png', 0, N'Other', N'90731d235980930c94262bf0728b75e9', CAST(N'2026-04-29T15:04:39.697' AS DateTime), 0, N'Khác', N'Khác')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (30, 11, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/0d1af492-258d-4df5-b87d-10f5bac47db3.jpg', 0, N'Other', N'5b3960445d98d47513ae2a312cd63dcd', CAST(N'2026-04-29T15:04:39.700' AS DateTime), 0, N'Khác', N'Khác')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (36, 14, N'/uploads/Cars/HONDA/HONDA_Honda_CR-V/9182ab1d-d0c5-407c-995f-21b14edda9e9.png', 0, N'Overview', N'93b3e8df36704cddd77ff6ef48c329af', CAST(N'2026-05-11T18:24:28.253' AS DateTime), 0, N'Honda CR-V 2026 thế hệ thứ 6 kiến tạo chuẩn mực mới cho dòng SUV đô thị. Sở hữu ngôn ngữ thiết kế thể thao đột phá, không gian cabin 7 chỗ rộng rãi tiện nghi, hệ thống an toàn chủ động Honda SENSING tiên tiến cùng tùy chọn động cơ xăng Turbo mạnh mẽ và động cơ Hybrid (e:HEV) siêu tiết kiệm nhiên liệu.', N'Honda CR-V 2026')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (37, 14, N'/uploads/Cars/HONDA/HONDA_Honda_CR-V/8ec5caa3-cf3c-406e-b56a-57f489ee3cf8.png', 0, N'Overview', N'8043ff09ec3ae84e706baf2e57e70aba', CAST(N'2026-05-11T18:24:28.277' AS DateTime), 0, N'Cập nhật giá xe Honda CR-V 2026 mới nhất kèm thông số kỹ thuật, hình ảnh thực tế và chương trình ưu đãi hấp dẫn. Đăng ký lái thử trải nghiệm ngay hôm nay!', N'Honda CR-V 2026')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (38, 14, N'/uploads/Cars/HONDA/HONDA_Honda_CR-V/18412935-2a74-49a3-ba4b-af4cae18faaa.png', 0, N'Exterior', N'8672dde38974e50bffe62ebd69fed80a', CAST(N'2026-05-11T18:24:28.283' AS DateTime), 0, N'Sở hữu ngôn ngữ thiết kế mới, Honda CR-V 2026 toát lên vẻ ngoài mạnh mẽ, thể thao nhưng không kém phần thanh lịch.

Cụm đèn trước LED: Sắc sảo, tích hợp đèn chạy ban ngày LED và tính năng tự động bật/tắt, tự động điều chỉnh góc chiếu.

Lưới tản nhiệt: Kích thước lớn, họa tiết hình khối lục giác đen bóng mạnh mẽ (trên bản RS).

Đuôi xe: Nổi bật với cụm đèn hậu LED chữ L điệu đà, mang tính nhận diện cao.

Cốp điện rảnh tay: Tiện lợi với tính năng đá cốp và tự động đóng khi bước ra xa (Walk-away close).', NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (39, 14, N'/uploads/Cars/HONDA/HONDA_Honda_CR-V/2dcc83af-fdcd-4369-81e9-b56d35640b39.png', 0, N'Interior', N'cd364a51bebba2fc6fa3ae51c9a72ad5', CAST(N'2026-05-11T18:24:28.297' AS DateTime), 0, N'Không gian cabin được nâng cấp vật liệu cao cấp, mang đến sự tinh tế và thoải mái tối đa cho mọi hành khách trên cả 3 hàng ghế (cấu hình 5+2).

Màn hình giải trí 9 inch: Cảm ứng sắc nét, hỗ trợ Apple CarPlay không dây và Android Auto.', NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (40, 14, N'/uploads/Cars/HONDA/HONDA_Honda_CR-V/e95a5ebe-9fe5-463a-98c3-502538fceb04.png', 0, N'Interior', N'f4efc59a748a7e4f9c363e3291de8a97', CAST(N'2026-05-11T18:24:28.307' AS DateTime), 0, N'Không gian cabin được nâng cấp vật liệu cao cấp, mang đến sự tinh tế và thoải mái tối đa cho mọi hành khách trên cả 3 hàng ghế (cấu hình 5+2).

Cửa sổ trời toàn cảnh Panorama: Mang lại không gian thoáng đãng, gần gũi với thiên nhiên.', NULL)
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (41, 14, N'/uploads/Cars/HONDA/HONDA_Honda_CR-V/a73019e6-ebd8-468d-ae8f-070d19beaf19.png', 0, N'Safety', N'dc3bc00581adf8e9f3958d725613bab9', CAST(N'2026-05-11T18:24:28.310' AS DateTime), 0, N'Tất cả các phiên bản Honda CR-V 2026 đều được trang bị hệ thống an toàn chủ động tiên tiến Honda SENSING, giúp bạn an tâm trên mọi hành trình:

Phanh giảm thiểu va chạm (CMBS)

Kiểm soát hành trình thích ứng bao gồm dải tốc độ thấp (ACC with LSF)  

Hỗ trợ giữ làn đường (LKAS)  

Giảm thiểu chệch làn đường (RDM)  

Đèn pha thích ứng tự động (AHB)  

Thông báo xe phía trước khởi hành (LCDN)

Ngoài ra, xe còn được trang bị: Camera quan sát làn đường LaneWatch, Camera 360 độ (bản cao cấp), 8 túi khí, cảm biến đỗ xe trước/sau, hỗ trợ đổ đèo (HDC),...', N'An Toàn Tuyệt Đối với Honda SENSING')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (42, 14, N'/uploads/Cars/HONDA/HONDA_Honda_CR-V/b930aadf-1f1f-4a99-9c44-686473e8f14e.png', 0, N'Performance', N'ad816ffe7cdc6819b011c2de27946451', CAST(N'2026-05-11T18:24:28.317' AS DateTime), 0, N'Honda CR-V 2026 cung cấp 2 tùy chọn động cơ đáp ứng mọi nhu cầu:

Động cơ 1.5L VTEC TURBO (Xăng): Sản sinh công suất 188 mã lực, mô-men xoắn 240 Nm, kết hợp hộp số CVT cho khả năng tăng tốc mượt mà và tiết kiệm. Có tùy chọn dẫn động 2 cầu AWD.

Hệ thống Hybrid e:HEV (RS): Sự kết hợp hoàn hảo giữa động cơ xăng 2.0L và mô-tơ điện, tổng công suất 204 mã lực, mang lại khả năng vận hành cực kỳ êm ái, mạnh mẽ ở dải tốc độ thấp và siêu tiết kiệm nhiên liệu (chỉ hơn 5 lít/100km).', N'Vận Hành Mạnh Mẽ và Tiết Kiệm')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (43, 15, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_8/0b1aec02-2671-47b2-b5bf-1fa4a0bdaaa7.png', 0, N'Overview', N'5feb0b433947c8f495151b9ce61f988e', CAST(N'2026-05-11T18:45:42.977' AS DateTime), 0, N'VinFast VF 8 khẳng định vị thế tiên phong trong kỷ nguyên di chuyển xanh với thiết kế sang trọng đẳng cấp châu Âu, khả năng vận hành mạnh mẽ tương đương các dòng xe thể thao và hệ thống công nghệ thông minh vượt trội phân khúc.', N'VinFast VF 8 2026 - Đẳng Cấp SUV Điện Thông Minh')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (44, 15, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_8/5ee98bc3-a34e-4c83-99d4-5555fc0ad2ab.png', 0, N'Exterior', N'64c56a7ff2f659caaa8891d400b371ad', CAST(N'2026-05-11T18:45:42.983' AS DateTime), 0, N'Sự kết hợp tinh tế giữa kiểu dáng SUV và những đường cong coupe mềm mại mang lại cho VinFast VF 8 tính khí động học tối ưu và vẻ ngoài vô cùng cuốn hút.

Đầu xe ấn tượng: Dải đèn LED định vị cánh chim đặc trưng ôm trọn logo chữ V nổi bật.

Đèn pha LED Matrix: Tự động thích ứng thông minh mang lại tầm nhìn hoàn hảo ban đêm.', N'Thiết Kế Thời Thượng Từ Pininfarina')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (45, 15, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_8/aa488954-3289-4712-8626-85ce7367ae7d.png', 0, N'Exterior', N'1abe8be0b32b4d2fc2af9a69b3171a83', CAST(N'2026-05-11T18:45:42.990' AS DateTime), 0, N'Sự kết hợp tinh tế giữa kiểu dáng SUV và những đường cong coupe mềm mại mang lại cho VinFast VF 8 tính khí động học tối ưu và vẻ ngoài vô cùng cuốn hút.

Đuôi xe năng động: Đường vuốt tinh xảo cùng dải LED chạy dài nguyên khối thể thao.', N'Thiết Kế Thời Thượng Từ Pininfarina')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (46, 15, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_8/ec871863-d608-47d4-98bc-6a1fcea44924.png', 0, N'Interior', N'95f6923f5f3e3bca5b48d82184e462a6', CAST(N'2026-05-11T18:45:42.997' AS DateTime), 0, N'Nội thất VF 8 được tối giản hóa tối đa, tập trung toàn bộ trải nghiệm vào người lái và hành khách nhờ cắt bỏ cụm đồng hồ truyền thống phía sau vô lăng.

Màn hình cảm ứng 15.6 inch: Trung tâm điều khiển mọi tính năng từ định vị, giải trí, điều hòa đến thiết lập xe.

Màn hình hiển thị kính lái HUD: Giúp người lái dễ dàng theo dõi tốc độ, bản đồ dẫn đường mà không cần rời mắt khỏi lộ trình.

Hệ thống loa cao cấp: Mang lại không gian âm nhạc sống động, chân thực.', N'Nội thất')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (47, 15, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_8/9baffb75-8a11-47e4-81f5-36a45ba9b066.png', 0, N'Safety', N'e959c48e27802dd7ef5b67416af32d71', CAST(N'2026-05-11T18:45:43.003' AS DateTime), 0, N'VinFast VF 8 được ví như một "ngôi nhà thông minh di động" nhờ tích hợp hàng loạt công nghệ tương lai:

Hệ thống ADAS: Hỗ trợ giữ làn, tự động phanh khẩn cấp, kiểm soát hành trình thích ứng, hỗ trợ lái xe trên đường cao tốc và đỗ xe thông minh.

Trợ lý ảo tiếng Việt: Cho phép điều khiển bằng giọng nói tự nhiên để điều chỉnh điều hòa, đóng mở cốp, nghe nhạc, đọc tin tức hay thậm chí là tán gẫu.

Dịch vụ thông minh Smart Services: Mua sắm trực tuyến, đặt lịch dịch vụ, nhận thông báo lỗi tự động và cập nhật phần mềm xe từ xa (FOTA).', N'Công Nghệ Hỗ Trợ Lái ADAS & Trợ Lý Ảo Thông Minh')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (48, 15, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_8/a96f6e03-2af6-4bc3-95ed-88bc01903c5b.png', 0, N'Performance', N'c0d5700c03fe4c0ecd8c963078c68f7f', CAST(N'2026-05-11T18:45:43.010' AS DateTime), 0, N'Trang bị hệ thống hai động cơ điện đặt trên hai cầu (AWD), VF 8 cho công suất tối đa lên tới 402 mã lực và mô-men xoắn 620 Nm (bản Plus), cho khả năng tăng tốc từ 0 - 100 km/h chỉ trong vòng 5.5 giây – ngang ngửa với nhiều mẫu xe thể thao hạng sang cỡ lớn.', N'Sức Mạnh Vận Hành Vượt Trội')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (49, 16, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_7/868c5095-8448-4f0d-87f9-692b77182625.png', 0, N'Overview', N'60695f9986ae93a5e4224fbc4293d995', CAST(N'2026-05-11T19:07:11.280' AS DateTime), 0, N'VinFast VF 7 là tuyên ngôn cá tính mạnh mẽ dành cho thế hệ trẻ năng động. Với ngôn ngữ thiết kế bất đối xứng mang hơi thở vũ trụ độc đáo cùng hiệu năng vận hành bùng nổ, VF 7 tái định nghĩa hoàn toàn trải nghiệm lái xe điện phân khúc C-SUV.', N'VinFast VF 7 2026 - Đột Phá Thiết Kế, Bứt Phá Công Nghệ')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (50, 16, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_7/00d422e0-43d1-4280-8d04-930d99279f0b.png', 0, N'Exterior', N'0d2abebd5fb7b7fb051661e461b203d0', CAST(N'2026-05-11T19:07:11.290' AS DateTime), 0, N'Được chắp bút bởi studio thiết kế danh tiếng Torino Design, VinFast VF 7 sở hữu diện mạo đầy táo bạo, cá tính và đậm chất khí động học.

Đầu xe futuristic: Dải đèn LED định vị hình cánh chim đặc trưng của VinFast, kết hợp cùng cụm đèn pha LED đặt thấp sắc sảo.

Đường nét cơ bắp: Thân xe dập nổi khỏe khoắn, tay nắm cửa ẩn hiện đại giúp tối ưu hóa luồng khí động học xung quanh xe.

Đuôi xe đầy lôi cuốn: Cụm đèn hậu LED thanh mảnh chạy dài ôm trọn đuôi xe cùng cánh lướt gió thể thao cá tính.', N'Ngoại Thất Độc Bản - Phong Cách Phi Thuyền Vũ Trụ')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (51, 16, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_7/ae77cf18-2ce3-4cf0-bbc9-ec5a0f39fefa.png', 0, N'Interior', N'b0d24cb31fbfce0065360af5d5d7b97b', CAST(N'2026-05-11T19:07:11.297' AS DateTime), 0, N'Nội thất của VF 7 được thiết kế tối giản, tập trung tối đa vào trải nghiệm của người lái với bệ tỳ tay trung tâm dốc nhẹ và màn hình cảm ứng xoay hướng về phía ghế lái.

Màn hình giải trí trung tâm 12.9 inch: Sắc nét, tích hợp mượt mà mọi tác vụ giải trí, định vị bản đồ và cài đặt hệ thống xe.

Màn hình hiển thị thông tin HUD: Giúp người lái dễ dàng kiểm soát tốc độ và các cảnh báo an toàn ngay trên kính lái mà không cần cúi xuống nhìn bảng đồng hồ.

Ghế da cao cấp: Thiết kế ôm sát cơ thể mang phong cách xe thể thao cùng tùy chọn phối màu nội thất phá cách.', N'Khoang Lái "Driver-centric" Hướng Về Người Lái')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (52, 16, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_7/4b7e3e37-fd97-4f92-9d2a-2a2d3ddb5048.png', 0, N'Safety', N'92518970bcb69263bdb9abd977ab5997', CAST(N'2026-05-11T19:07:11.303' AS DateTime), 0, N'Sở hữu hệ thống an toàn chủ động ADAS và dịch vụ thông minh Smart Services cao cấp:

Hệ thống lái ADAS: Hỗ trợ giữ làn đường, kiểm soát hành trình thích ứng, phanh khẩn cấp tự động và cảnh báo điểm mù thông minh.

Trợ lý ảo VinFast: Khả năng nhận diện giọng nói tiếng Việt cực nhạy, hỗ trợ điều chỉnh điều hòa, hỏi đáp thông tin, mở nhạc rảnh tay tiện lợi.

Tính năng FOTA: Tự động cập nhật và tối ưu hóa hệ thống phần mềm của xe từ xa mà không cần phải mang xe đến xưởng dịch vụ.', N'Công Nghệ Thông Minh và An Toàn Đỉnh Cao')
GO
INSERT [dbo].[CarImages] ([CarImageId], [CarId], [ImageUrl], [IsMainImage], [ImageType], [FileHash], [CreatedAt], [Is360Degree], [Description], [Title]) VALUES (53, 16, N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_7/7916333c-29f5-4cca-ac7e-c42c85820e3a.png', 0, N'Performance', N'c03998248efed3d1b7b968d897429692', CAST(N'2026-05-11T19:07:11.310' AS DateTime), 0, N'Không chỉ đẹp, VF 7 còn là "quái thú" thực sự trên đường phố.

Bản Plus: Trang bị hệ thống động cơ điện kép (AWD) cho công suất cực đại lên tới 349 mã lực và mô-men xoắn 500 Nm, giúp xe bứt tốc từ 0 - 100 km/h cực kỳ phấn khích.

Hệ thống pin công nghệ cao: Cho quãng đường di chuyển ấn tượng lên tới 431 km cho một lần sạc đầy (theo chuẩn WLTP), đáp ứng hoàn hảo mọi nhu cầu di chuyển hàng ngày hay những chuyến đi dã ngoại cuối tuần.', N'Khả Năng Vận Hành Mạnh Mẽ Vượt Trội')
GO
SET IDENTITY_INSERT [dbo].[CarImages] OFF
GO
SET IDENTITY_INSERT [dbo].[CarInventories] ON 
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (14, 6, 10, 4, N'OnDisplay', CAST(N'2026-04-29T12:23:46.3634230' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (22, 3, 5, 2, N'OnDisplay', CAST(N'2026-04-30T23:38:34.2253777' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (23, 3, 6, 1, N'OnDisplay', CAST(N'2026-04-30T23:38:39.5972215' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (24, 4, 7, 5, N'OnDisplay', CAST(N'2026-04-30T23:38:45.5258327' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (25, 4, 8, 2, N'OnDisplay', CAST(N'2026-04-30T23:38:51.6717609' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (26, 5, 9, 2, N'OnDisplay', CAST(N'2026-04-30T23:38:56.6927079' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (63, 1, 11, 5, N'OnDisplay', CAST(N'2026-05-08T01:04:13.1872373' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (64, 7, 11, 2, N'OnDisplay', CAST(N'2026-05-08T01:04:13.1939349' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (65, 5, 14, 5, N'OnDisplay', CAST(N'2026-05-11T18:24:28.1724299' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (68, 5, 16, 1, N'OnDisplay', CAST(N'2026-05-11T19:07:11.2378527' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (69, 3, 2, 8, N'OnDisplay', CAST(N'2026-05-11T19:08:37.7557129' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (70, 3, 3, 6, N'OnDisplay', CAST(N'2026-05-11T19:08:49.5665202' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (71, 3, 4, 3, N'OnDisplay', CAST(N'2026-05-11T19:09:01.1649110' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (72, 1, 15, 10, N'OnDisplay', CAST(N'2026-05-11T19:09:34.3897981' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (77, 5, 121, 1, N'OnDisplay', CAST(N'2026-05-11T23:55:18.1644613' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (78, 5, 122, 1, N'OnDisplay', CAST(N'2026-05-11T23:57:32.9090365' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (79, 5, 123, 1, N'OnDisplay', CAST(N'2026-05-11T23:58:08.4288449' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (80, 5, 124, 1, N'OnDisplay', CAST(N'2026-05-11T23:59:10.8149553' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (81, 5, 125, 1, N'OnDisplay', CAST(N'2026-05-11T23:59:46.6806214' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (83, 5, 126, 1, N'OnDisplay', CAST(N'2026-05-12T00:00:44.4676823' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (84, 5, 127, 1, N'OnDisplay', CAST(N'2026-05-12T00:01:08.1511234' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (85, 5, 128, 1, N'OnDisplay', CAST(N'2026-05-12T00:01:53.8536152' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (86, 5, 129, 1, N'OnDisplay', CAST(N'2026-05-12T00:02:24.3070624' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (87, 5, 130, 1, N'OnDisplay', CAST(N'2026-05-12T00:02:56.1783398' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (88, 5, 131, 1, N'OnDisplay', CAST(N'2026-05-12T00:03:57.0375997' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (89, 5, 132, 1, N'OnDisplay', CAST(N'2026-05-12T00:04:28.4742199' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (90, 5, 133, 1, N'OnDisplay', CAST(N'2026-05-12T00:05:05.0334268' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (91, 5, 134, 1, N'OnDisplay', CAST(N'2026-05-12T00:05:43.0993775' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (92, 5, 135, 1, N'OnDisplay', CAST(N'2026-05-12T00:06:12.1362425' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (95, 5, 137, 1, N'OnDisplay', CAST(N'2026-05-12T00:07:34.3111303' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (96, 5, 136, 1, N'OnDisplay', CAST(N'2026-05-12T00:07:40.9109629' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (97, 5, 138, 1, N'OnDisplay', CAST(N'2026-05-12T00:09:19.4612446' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (98, 5, 139, 1, N'OnDisplay', CAST(N'2026-05-12T00:09:46.4697730' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (99, 5, 140, 1, N'OnDisplay', CAST(N'2026-05-12T00:10:15.5014578' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (100, 5, 141, 1, N'OnDisplay', CAST(N'2026-05-12T00:11:37.2596455' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (101, 5, 142, 1, N'OnDisplay', CAST(N'2026-05-12T00:12:01.7644111' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (102, 5, 143, 1, N'OnDisplay', CAST(N'2026-05-12T00:12:30.0204000' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (103, 5, 144, 1, N'OnDisplay', CAST(N'2026-05-12T00:13:09.8435131' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (104, 5, 145, 1, N'OnDisplay', CAST(N'2026-05-12T00:13:35.6734561' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (105, 5, 146, 1, N'OnDisplay', CAST(N'2026-05-12T00:14:08.7754199' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (106, 5, 147, 1, N'OnDisplay', CAST(N'2026-05-12T00:14:41.3924392' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (107, 5, 148, 1, N'OnDisplay', CAST(N'2026-05-12T00:15:15.3135771' AS DateTime2), NULL)
GO
INSERT [dbo].[CarInventories] ([InventoryId], [ShowroomId], [CarId], [Quantity], [DisplayStatus], [UpdatedAt], [CarColorId]) VALUES (108, 5, 149, 1, N'OnDisplay', CAST(N'2026-05-12T00:15:43.0976392' AS DateTime2), NULL)
GO
SET IDENTITY_INSERT [dbo].[CarInventories] OFF
GO
SET IDENTITY_INSERT [dbo].[CarPricingVersions] ON 
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (47, 11, N'VF 9 Plus (Thuê pin)', CAST(1589000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-08T01:04:13.250' AS DateTime), CAST(N'2026-05-08T01:04:13.250' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (48, 11, N'VF 9 Plus (Kèm pin)', CAST(2088000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-08T01:04:13.250' AS DateTime), CAST(N'2026-05-08T01:04:13.250' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (49, 14, N'Honda CR-V G', CAST(1039000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-11T18:24:28.213' AS DateTime), CAST(N'2026-05-11T18:24:28.213' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (50, 14, N'Honda CR-V L', CAST(1099000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-11T18:24:28.227' AS DateTime), CAST(N'2026-05-11T18:24:28.227' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (51, 14, N'Honda CR-V L AWD', CAST(1250000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-11T18:24:28.230' AS DateTime), CAST(N'2026-05-11T18:24:28.230' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (60, 16, N'VinFast VF 7 Base (Thuê pin)', CAST(850000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-11T19:07:11.257' AS DateTime), CAST(N'2026-05-11T19:07:11.257' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (61, 16, N'VinFast VF 7 Base (Mua pin)', CAST(999000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-11T19:07:11.260' AS DateTime), CAST(N'2026-05-11T19:07:11.260' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (62, 16, N'VinFast VF 7 Plus (Thuê pin)', CAST(999000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-11T19:07:11.263' AS DateTime), CAST(N'2026-05-11T19:07:11.263' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (63, 16, N'VinFast VF 7 Plus (Mua  pin)', CAST(1199000000.00 AS Decimal(18, 2)), 4, 1, CAST(N'2026-05-11T19:07:11.267' AS DateTime), CAST(N'2026-05-11T19:07:11.267' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (64, 15, N'VinFast VF 8 Eco (Thuê pin)', CAST(1110000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:09:34.437' AS DateTime), CAST(N'2026-05-12T02:09:34.437' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (65, 15, N'VinFast VF 8 Eco (Mua pin)', CAST(1320000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:09:34.437' AS DateTime), CAST(N'2026-05-12T02:09:34.437' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (66, 15, N'VinFast VF 8 Plus (Thuê pin)', CAST(1290000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:09:34.437' AS DateTime), CAST(N'2026-05-12T02:09:34.437' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (67, 15, N'VinFast VF 8 Plus (Mua pin)', CAST(1510000000.00 AS Decimal(18, 2)), 4, 1, CAST(N'2026-05-12T02:09:34.437' AS DateTime), CAST(N'2026-05-12T02:09:34.437' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (101, 101, N'2.0G', CAST(1070000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (102, 101, N'2.0Q', CAST(1185000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (103, 101, N'2.5Q', CAST(1370000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (104, 101, N'2.5HEV', CAST(1460000000.00 AS Decimal(18, 2)), 4, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (105, 102, N'1.8G', CAST(820000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (106, 102, N'1.8V', CAST(860000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (107, 102, N'1.8HEV', CAST(905000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (108, 103, N'2.4 MT 4x2', CAST(1026000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (109, 103, N'2.4 AT 4x2', CAST(1118000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (110, 103, N'Legender 2.8 AT 4x4', CAST(1470000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (111, 104, N'MT Tiêu chuẩn', CAST(426000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (112, 104, N'MT', CAST(472000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (113, 104, N'AT', CAST(501000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (114, 104, N'AT Đặc biệt', CAST(542000000.00 AS Decimal(18, 2)), 4, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (115, 105, N'2.0 Xăng Tiêu chuẩn', CAST(769000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (116, 105, N'2.0 Xăng Đặc biệt', CAST(839000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (117, 105, N'2.0 Dầu Đặc biệt', CAST(869000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (118, 105, N'1.6 Turbo', CAST(899000000.00 AS Decimal(18, 2)), 4, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (119, 106, N'2.5 Xăng Tiêu Chuẩn', CAST(1029000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (120, 106, N'2.2 Dầu Tiêu Chuẩn', CAST(1120000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (121, 106, N'2.5 Xăng Cao Cấp', CAST(1210000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (122, 106, N'Hybrid', CAST(1369000000.00 AS Decimal(18, 2)), 4, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (123, 107, N'1.5L AT', CAST(599000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (124, 107, N'1.5L Luxury', CAST(679000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (125, 107, N'1.5L Premium', CAST(739000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (126, 107, N'1.5T GT-Line', CAST(839000000.00 AS Decimal(18, 2)), 4, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (127, 108, N'1.6 Deluxe MT', CAST(539000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (128, 108, N'1.6 Luxury', CAST(569000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (129, 108, N'1.6 Premium', CAST(599000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (130, 108, N'2.0 Premium', CAST(639000000.00 AS Decimal(18, 2)), 4, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (131, 109, N'2.2D Luxury 8 ghế', CAST(1189000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (132, 109, N'2.2D Premium 8 ghế', CAST(1269000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (133, 109, N'2.2D Signature 7 ghế', CAST(1389000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (134, 109, N'3.5G Signature 7 ghế', CAST(1759000000.00 AS Decimal(18, 2)), 4, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (135, 110, N'MT', CAST(560000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (136, 110, N'AT', CAST(598000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (137, 110, N'AT Premium', CAST(658000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (138, 110, N'Cross', CAST(698000000.00 AS Decimal(18, 2)), 4, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (139, 111, N'GLX', CAST(599000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (140, 111, N'Exceed', CAST(640000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (141, 111, N'Premium', CAST(680000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (142, 111, N'Ultimate', CAST(705000000.00 AS Decimal(18, 2)), 4, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (143, 112, N'Ambiente 2.0L AT 4x2', CAST(1099000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (144, 112, N'Sport 2.0L AT 4x2', CAST(1178000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (145, 112, N'Titanium 2.0L AT 4x2', CAST(1299000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (146, 112, N'Titanium+ 2.0L AT 4x4', CAST(1468000000.00 AS Decimal(18, 2)), 4, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (147, 113, N'Trend', CAST(799000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (148, 113, N'Titanium', CAST(889000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (149, 113, N'Titanium X', CAST(929000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (150, 114, N'1.5L Deluxe', CAST(579000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (151, 114, N'1.5L Luxury', CAST(619000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (152, 114, N'1.5L Premium', CAST(729000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (153, 115, N'Luxury', CAST(949000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (154, 115, N'Premium', CAST(1024000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (155, 115, N'Premium AWD', CAST(1119000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (156, 116, N'G', CAST(559000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (157, 116, N'L', CAST(589000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (158, 116, N'RS', CAST(609000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (159, 117, N'E', CAST(730000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (160, 117, N'G', CAST(770000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (161, 117, N'RS', CAST(870000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (162, 118, N'2.0 i-L', CAST(969000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (163, 118, N'2.0 i-L EyeSight', CAST(1099000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (164, 118, N'2.0 i-S EyeSight', CAST(1199000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (165, 119, N'Active', CAST(894000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (166, 119, N'Allure', CAST(994000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (167, 119, N'GT', CAST(1084000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (168, 120, N'GLX AT', CAST(599000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (169, 120, N'Sport Limited', CAST(639000000.00 AS Decimal(18, 2)), 2, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (170, 120, N'Hybrid', CAST(660000000.00 AS Decimal(18, 2)), 3, 1, CAST(N'2026-05-12T02:25:43.740' AS DateTime), CAST(N'2026-05-12T02:25:43.740' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (230, 150, N'1.4 AT', CAST(470000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (231, 151, N'1.5L Luxury', CAST(499000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (232, 152, N'2.0L Premium', CAST(769000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (233, 153, N'1.5L Premium', CAST(589000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (234, 154, N'2.0L Luxury', CAST(699000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (235, 155, N'1.9 AT', CAST(584000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (236, 156, N'2.5L Turbo', CAST(1200000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (237, 157, N'MHEV 3.3L', CAST(1500000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (238, 158, N'Turbo S', CAST(2000000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (239, 159, N'RF 2.0L', CAST(2000000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (240, 160, N'2.5L AWD', CAST(800000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (241, 161, N'CVT Premium', CAST(490000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (242, 162, N'2.0 CVT Premium', CAST(950000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (243, 163, N'4x2 AT', CAST(650000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (244, 164, N'4x4 AT', CAST(1365000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (245, 165, N'Premium', CAST(698000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (246, 166, N'CVT Eco', CAST(350000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (247, 167, N'2.4 MIVEC', CAST(300000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (248, 168, N'2.4 GLS', CAST(200000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (249, 169, N'1.6 MT', CAST(400000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (250, 170, N'1.5 Turbo', CAST(900000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (251, 171, N'2.0L V6 4x4', CAST(1299000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (252, 172, N'2.3 Ecoboost', CAST(2399000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (253, 173, N'Mid', CAST(849000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (254, 174, N'1.5L Titanium', CAST(450000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (255, 175, N'1.5 Ecoboost', CAST(1100000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (256, 176, N'Outer Banks', CAST(3000000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (257, 177, N'GT 5.0 V8', CAST(3500000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (258, 178, N'Lariat', CAST(4000000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (259, 179, N'ST-Line', CAST(750000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (260, 180, N'ST', CAST(1800000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (261, 181, N'G Tiêu chuẩn', CAST(699000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (262, 182, N'G Sensing', CAST(661000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (263, 183, N'1.5 Turbo', CAST(1319000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (264, 184, N'RS', CAST(380000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (265, 185, N'RS', CAST(450000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (266, 186, N'Touring', CAST(3500000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (267, 187, N'Elite', CAST(2500000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (268, 188, N'RTL-E', CAST(3000000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (269, 189, N'TrailSport', CAST(2800000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (270, 190, N'RS', CAST(550000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (271, 191, N'M Sport', CAST(1399000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (272, 192, N'xDrive40i', CAST(4019000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (273, 193, N'Plus', CAST(1599000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (274, 194, N'AMG', CAST(2799000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (275, 195, N'45 TFSI', CAST(2400000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (276, 196, N'Premium', CAST(3430000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (277, 197, N'Standard', CAST(3350000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (278, 198, N'B6 AWD', CAST(2320000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (279, 199, N'Active', CAST(709000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (280, 200, N'1.5L STD', CAST(399000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T02:32:26.780' AS DateTime), CAST(N'2026-05-12T02:32:26.780' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (285, 121, N'1.0 Turbo', CAST(552000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T06:55:18.180' AS DateTime), CAST(N'2026-05-12T06:55:18.180' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (286, 122, N'CVT Top', CAST(698000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T06:57:32.937' AS DateTime), CAST(N'2026-05-12T06:57:32.937' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (287, 123, N'HEV Hybrid', CAST(990000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T06:58:08.440' AS DateTime), CAST(N'2026-05-12T06:58:08.440' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (288, 124, N'1.5 Xăng', CAST(730000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T06:59:10.840' AS DateTime), CAST(N'2026-05-12T06:59:10.840' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (289, 125, N'LC300 VIP', CAST(4286000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T06:59:46.710' AS DateTime), CAST(N'2026-05-12T06:59:46.710' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (291, 126, N'2.4L 4x2 AT', CAST(668000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:00:44.480' AS DateTime), CAST(N'2026-05-12T07:00:44.480' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (292, 127, N'1.2G AT', CAST(405000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:01:08.173' AS DateTime), CAST(N'2026-05-12T07:01:08.173' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (293, 128, N'CVT', CAST(598000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:01:53.880' AS DateTime), CAST(N'2026-05-12T07:01:53.880' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (294, 129, N'Luxury HEV', CAST(4470000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:02:24.320' AS DateTime), CAST(N'2026-05-12T07:02:24.320' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (295, 130, N'VX', CAST(2628000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:02:56.207' AS DateTime), CAST(N'2026-05-12T07:02:56.207' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (296, 131, N'1.2 AT', CAST(435000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:03:57.067' AS DateTime), CAST(N'2026-05-12T07:03:57.067' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (297, 132, N'1.5 Đặc biệt', CAST(650000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:04:28.487' AS DateTime), CAST(N'2026-05-12T07:04:28.487' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (298, 133, N'X Tiêu chuẩn', CAST(575000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:05:05.063' AS DateTime), CAST(N'2026-05-12T07:05:05.063' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (299, 134, N'1.5T Đặc biệt', CAST(850000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:05:43.110' AS DateTime), CAST(N'2026-05-12T07:05:43.110' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (300, 135, N'Prestige 6 ghế', CAST(1469000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:06:12.163' AS DateTime), CAST(N'2026-05-12T07:06:12.163' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (303, 137, N'1.0 T-GDi', CAST(539000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:07:34.337' AS DateTime), CAST(N'2026-05-12T07:07:34.337' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (304, 136, N'1.6 AT', CAST(599000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:07:40.920' AS DateTime), CAST(N'2026-05-12T07:07:40.920' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (305, 138, N'Premium', CAST(1500000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:09:19.493' AS DateTime), CAST(N'2026-05-12T07:09:19.493' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (306, 139, N'Prestige 72.6 kWh', CAST(1300000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:09:46.483' AS DateTime), CAST(N'2026-05-12T07:09:46.483' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (307, 140, N'2.0 AT', CAST(636000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:10:18.557' AS DateTime), CAST(N'2026-05-12T07:10:18.557' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (308, 141, N'AT X-Line', CAST(424000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:11:37.293' AS DateTime), CAST(N'2026-05-12T07:11:37.293' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (309, 142, N'1.5 Luxury', CAST(574000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:12:01.777' AS DateTime), CAST(N'2026-05-12T07:12:01.777' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (310, 143, N'1.5G IVT', CAST(629000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:12:30.050' AS DateTime), CAST(N'2026-05-12T07:12:30.050' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (311, 144, N'2.0G Premium', CAST(799000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:13:09.857' AS DateTime), CAST(N'2026-05-12T07:13:09.857' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (312, 145, N'2.2D Luxury', CAST(964000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:13:35.707' AS DateTime), CAST(N'2026-05-12T07:13:35.707' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (313, 146, N'2.0 Luxury', CAST(859000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:14:08.787' AS DateTime), CAST(N'2026-05-12T07:14:08.787' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (314, 147, N'AWD', CAST(2000000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:14:41.423' AS DateTime), CAST(N'2026-05-12T07:14:41.423' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (315, 148, N'AT Deluxe', CAST(414000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:15:15.323' AS DateTime), CAST(N'2026-05-12T07:15:15.323' AS DateTime))
GO
INSERT [dbo].[CarPricingVersions] ([PricingVersionId], [CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (316, 149, N'2.0 GAT', CAST(559000000.00 AS Decimal(18, 2)), 1, 1, CAST(N'2026-05-12T07:15:47.217' AS DateTime), CAST(N'2026-05-12T07:15:47.217' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[CarPricingVersions] OFF
GO
SET IDENTITY_INSERT [dbo].[Cars] ON 
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (2, N'VinFast VF3', N'VF3', 2024, 0, CAST(281000000.00 AS Decimal(18, 2)), N'Xe điện mini đô thị, nhỏ gọn, dễ lái.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF3/60fe6542-f357-4445-b033-f13156d31845.webp', 2, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-05-11T19:08:37.737' AS DateTime), N'Điện', 0, CAST(N'2026-05-11T17:51:50.430' AS DateTime), 1, N'Số tự động', N'Hatchback', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (3, N'VinFast VF5', N'VF5', 2024, 0, CAST(497000000.00 AS Decimal(18, 2)), N'Xe điện gầm cao cỡ nhỏ, phù hợp gia đình.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF5/61a39adb-e5b9-46c7-833c-34dc1a1ffa70.webp', 2, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-05-11T19:08:49.557' AS DateTime), N'Điện', 0, CAST(N'2026-05-11T17:51:51.547' AS DateTime), 1, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (4, N'VinFast VF6', N'VF6', 2025, 0, CAST(647000000.00 AS Decimal(18, 2)), N'Xe điện cỡ C, tiện nghi và an toàn.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF6/5dc49b23-44d0-49ca-94e2-2547ae67064b.webp', 2, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-05-11T19:09:01.157' AS DateTime), N'Điện', 1, CAST(N'2026-05-11T17:51:51.967' AS DateTime), 1, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (5, N'VinFast VF7', N'VF7', 2025, 0, CAST(751000000.00 AS Decimal(18, 2)), N'SUV điện cỡ C thiết kế hiện đại.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.png', 2, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-30T23:38:34.217' AS DateTime), N'Điện', 1, CAST(N'2026-05-11T17:51:52.373' AS DateTime), 1, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (6, N'VinFast VF9', N'VF9', 2025, 0, CAST(1349000000.00 AS Decimal(18, 2)), N'SUV điện 7 chỗ cao cấp.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.png', 2, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-30T23:38:39.590' AS DateTime), N'Điện', 1, CAST(N'2026-05-11T17:51:52.710' AS DateTime), 1, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (7, N'Toyota Vios G', N'Vios G', 2024, 0, CAST(545000000.00 AS Decimal(18, 2)), N'Sedan bền bỉ, tiết kiệm, phù hợp dịch vụ.', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.png', 2, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-30T23:38:45.520' AS DateTime), N'Xăng', 1, CAST(N'2026-05-11T17:51:53.150' AS DateTime), 1, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (8, N'Toyota Corolla Cross', N'1.8V', 2024, 0, CAST(820000000.00 AS Decimal(18, 2)), N'Crossover đô thị, trang bị an toàn tốt.', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.png', 2, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-30T23:38:51.667' AS DateTime), N'Xăng', 1, CAST(N'2026-05-11T17:51:53.577' AS DateTime), 1, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (9, N'Ford Everest Titanium', N'Everest', 2024, 0, CAST(1450000000.00 AS Decimal(18, 2)), N'SUV 7 chỗ mạnh mẽ, đi đường dài tốt.', N'FORD', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.png', 2, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-30T23:38:56.687' AS DateTime), N'Dầu', 1, CAST(N'2026-05-11T17:51:56.017' AS DateTime), 1, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (10, N'Mitsubishi Xpander Cross', N'Xpander Cross', 2024, 0, CAST(698000000.00 AS Decimal(18, 2)), N'MPV gầm cao, rộng rãi, dễ sử dụng.', N'MITSUBISHI', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/default-car.png', 2, CAST(N'2026-04-28T17:54:32.737' AS DateTime), CAST(N'2026-04-29T14:46:58.970' AS DateTime), N'Xăng', 1, CAST(N'2026-05-11T17:51:56.590' AS DateTime), 1, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (11, N'VinFast VF 9 Plus', N'VF 9', 2026, 0, CAST(1589000000.00 AS Decimal(18, 2)), N'VinFast VF9 Plus có khả năng vận hành mạnh mẽ khi sử dụng động cơ điện cho công suất tối đa lên đến 300kW, mô men xoắn cực đại 620Nm. Đây là con số ấn tượng cho một chiếc xe ô tô điện, không hề thua kém các dòng xe sử dụng động cơ đốt trong. Thêm vào đó, với việc sử dụng hệ dẫn động 2 cầu toàn thời gian AWD, VF 9 Plus có độ bám đường vượt trội, xứng đáng là một lựa chọn lý tưởng cho những người đam mê tốc độ và chinh phục nhiều địa hình khác nhau.
    Một trong những hiểu lầm lớn của người dùng về ô tô điện đó là phạm vi di chuyển ngắn. Tuy nhiên, ô tô điện VF 9 Plus hứa hẹn sẽ xóa bỏ rào cản về phạm vi lái, giúp khách hàng an tâm hơn khi sở hữu một chiếc xe chạy điện có khả năng di chuyển quãng đường tối đa sau 1 lần sạc là 423km - điều kiện tiêu chuẩn châu Âu (WLTP). Theo đó, thời gian sạc đầy của pin là 26 phút.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_9_Plus/5eac9a4f-7258-48eb-af6f-60025fe5e41a.png', 2, CAST(N'2026-04-29T15:04:39.493' AS DateTime), CAST(N'2026-05-08T01:04:13.173' AS DateTime), N'Điện', 0, CAST(N'2026-05-07T21:11:02.823' AS DateTime), 3, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (14, N'Honda CR-V', N'G6', 2026, 0, CAST(1039000000.00 AS Decimal(18, 2)), N'Honda CR-V thế hệ thứ 6 sở hữu thiết kế thể thao, thanh lịch đột phá cùng không gian nội thất rộng rãi, tiện nghi vượt trội. Xe được trang bị hệ thống an toàn chủ động Honda SENSING tiên tiến trên tất cả các phiên bản, cùng tùy chọn động cơ Hybrid mạnh mẽ, tiết kiệm nhiên liệu tối ưu.', N'HONDA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/HONDA/HONDA_Honda_CR-V/3e0be00f-2f33-4c1e-9698-cbb7022d396d.png', 2, CAST(N'2026-05-11T18:24:28.113' AS DateTime), CAST(N'2026-05-11T18:24:28.233' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, 1)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (15, N'VinFast VF 8', N'VF 8', 2026, 0, CAST(1110000000.00 AS Decimal(18, 2)), N'VinFast VF 8 là dòng SUV điện phân khúc D đẳng cấp toàn cầu, kết hợp hoàn hảo giữa kiểu dáng thời thượng được chắp bút bởi Pininfarina và công nghệ hỗ trợ lái ADAS tiên tiến. Xe sở hữu động cơ điện kép mạnh mẽ cho khả năng tăng tốc ấn tượng, không gian nội thất tối giản, sang trọng và hệ thống trợ lý ảo thông minh vượt trội.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_8/0a0d84db-9df5-4fcf-956b-93a86c46d243.webp', 2, CAST(N'2026-05-11T18:45:42.907' AS DateTime), CAST(N'2026-05-11T19:09:34.353' AS DateTime), N'Điện', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, 1)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (16, N'VinFast VF 7', N'VF 7', 2026, 0, CAST(850000000.00 AS Decimal(18, 2)), N'VinFast VF 7 là mẫu SUV thuần điện phân khúc C mang phong cách thiết kế vũ trụ "Asymmetric Aerospace" cực kỳ cá tính và tương lai. Xe sở hữu động cơ mạnh mẽ vượt trội tầm phân khúc, khoang lái hướng về người lái hiện đại cùng hàng loạt công nghệ an toàn ADAS đỉnh cao.', N'VINFAST', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/VINFAST/VINFAST_VinFast_VF_7/511ae145-82b6-4f01-bf97-f3602c955328.webp', 2, CAST(N'2026-05-11T19:07:11.223' AS DateTime), CAST(N'2026-05-11T19:07:11.267' AS DateTime), N'Điện', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, 1)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (101, N'Toyota Camry 2024', N'Camry', 2024, 1, CAST(1070000000.00 AS Decimal(18, 2)), N'Sedan hạng D doanh nhân sang trọng, đẳng cấp.', N'Toyota', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Hybrid', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (102, N'Toyota Corolla Cross', N'Corolla Cross', 2024, 1, CAST(820000000.00 AS Decimal(18, 2)), N'CUV cỡ B/C đô thị gầm cao tiện dụng, siêu tiết kiệm.', N'Toyota', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Hybrid', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (103, N'Toyota Fortuner 2024', N'Fortuner', 2024, 1, CAST(1026000000.00 AS Decimal(18, 2)), N'SUV 7 chỗ gầm cao mạnh mẽ, bền bỉ cùng thời gian.', N'Toyota', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (104, N'Hyundai Accent 2024', N'Accent', 2024, 1, CAST(426000000.00 AS Decimal(18, 2)), N'Sedan hạng B thiết kế trẻ trung, nhiều option nhất phân khúc.', N'Hyundai', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (105, N'Hyundai Tucson 2024', N'Tucson', 2024, 1, CAST(769000000.00 AS Decimal(18, 2)), N'CUV 5 chỗ ngôn ngữ thiết kế Sensuous Sportiness.', N'Hyundai', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (106, N'Hyundai Santa Fe 2024', N'Santa Fe', 2024, 1, CAST(1029000000.00 AS Decimal(18, 2)), N'SUV 7 chỗ cao cấp, sang trọng, ngập tràn công nghệ.', N'Hyundai', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Hybrid', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (107, N'Kia Seltos 2024', N'Seltos', 2024, 1, CAST(599000000.00 AS Decimal(18, 2)), N'SUV cỡ B thể thao, thời trang, nội thất rộng rãi.', N'Kia', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (108, N'Kia K3 2024', N'K3', 2024, 1, CAST(539000000.00 AS Decimal(18, 2)), N'Sedan cỡ C giá tốt nhất phân khúc, kiểu dáng thể thao.', N'Kia', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (109, N'Kia Carnival 2024', N'Carnival', 2024, 1, CAST(1189000000.00 AS Decimal(18, 2)), N'SUV đô thị cỡ lớn (Minivan) chuẩn mực cho gia đình.', N'Kia', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (110, N'Mitsubishi Xpander', N'Xpander', 2024, 1, CAST(560000000.00 AS Decimal(18, 2)), N'MPV quốc dân 7 chỗ, bán chạy nhất phân khúc.', N'Mitsubishi', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (111, N'Mitsubishi Xforce', N'Xforce', 2024, 1, CAST(599000000.00 AS Decimal(18, 2)), N'Tân binh CUV cỡ B, thiết kế tương lai, âm thanh Yamaha.', N'Mitsubishi', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (112, N'Ford Everest 2024', N'Everest', 2024, 1, CAST(1099000000.00 AS Decimal(18, 2)), N'SUV 7 chỗ Mỹ cơ bắp, vận hành off-road đỉnh cao.', N'Ford', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (113, N'Ford Territory', N'Territory', 2024, 1, CAST(799000000.00 AS Decimal(18, 2)), N'CUV cỡ C hiện đại, rộng rãi, giá cạnh tranh.', N'Ford', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (114, N'Mazda 3 2024', N'Mazda 3', 2024, 1, CAST(579000000.00 AS Decimal(18, 2)), N'Sedan hạng C nghệ thuật Kodo, đẹp nhất phân khúc.', N'Mazda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (115, N'Mazda CX-8', N'CX-8', 2024, 1, CAST(949000000.00 AS Decimal(18, 2)), N'SUV 7 chỗ gia đình rộng rãi, êm ái, cách âm tốt.', N'Mazda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (116, N'Honda City 2024', N'City', 2024, 1, CAST(559000000.00 AS Decimal(18, 2)), N'Sedan hạng B lái hay nhất, có Honda Sensing.', N'Honda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (117, N'Honda Civic 2024', N'Civic', 2024, 1, CAST(730000000.00 AS Decimal(18, 2)), N'Sedan hạng C đậm chất thể thao, DNA đua xe.', N'Honda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (118, N'Subaru Forester', N'Forester', 2024, 1, CAST(969000000.00 AS Decimal(18, 2)), N'SUV nổi tiếng với động cơ Boxer và hệ dẫn động S-AWD.', N'Subaru', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (119, N'Peugeot 3008', N'3008', 2024, 1, CAST(894000000.00 AS Decimal(18, 2)), N'SUV châu Âu với buồng lái i-Cockpit như phi cơ bay.', N'Peugeot', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (120, N'Suzuki XL7', N'XL7', 2024, 1, CAST(599000000.00 AS Decimal(18, 2)), N'SUV 7 chỗ thực dụng, bền bỉ, tiết kiệm kinh tế.', N'Suzuki', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:25:43.720' AS DateTime), CAST(N'2026-05-12T02:25:43.720' AS DateTime), N'Hybrid', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (121, N'Toyota Raize', N'Raize', 2024, 1, CAST(552000000.00 AS Decimal(18, 2)), N'SUV cỡ A cỡ nhỏ gọn, gầm cao', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/TOYOTA/TOYOTA_Toyota_Raize/147c89ef-ad17-4744-b9e5-ec748038d0ef.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-11T23:55:18.140' AS DateTime), N'Xăng', 0, CAST(N'2026-05-11T22:55:26.997' AS DateTime), 1, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (122, N'Toyota Veloz Cross', N'Veloz', 2024, 1, CAST(698000000.00 AS Decimal(18, 2)), N'MPV lai SUV 7 chỗ tiện dụng', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/TOYOTA/TOYOTA_Toyota_Veloz_Cross/52675ce2-a58c-443d-8ca6-249e1b6a97cb.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-11T23:57:32.897' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (123, N'Toyota Innova Cross', N'Innova', 2024, 1, CAST(990000000.00 AS Decimal(18, 2)), N'MPV huyền thoại thế hệ mới', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/TOYOTA/TOYOTA_Toyota_Innova_Cross/0541c392-dedc-471a-908d-4d4678d363fd.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-11T23:58:08.423' AS DateTime), N'Hybrid', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (124, N'Toyota Yaris Cross', N'Yaris Cross', 2024, 1, CAST(730000000.00 AS Decimal(18, 2)), N'CUV cỡ B thiết kế năng động', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/TOYOTA/TOYOTA_Toyota_Yaris_Cross/8f89699c-1c5e-40cd-87e9-c4f9b5265c70.jpg', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-11T23:59:10.807' AS DateTime), N'Hybrid', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (125, N'Toyota Land Cruiser', N'Land Cruiser', 2024, 1, CAST(4286000000.00 AS Decimal(18, 2)), N'SUV full-size hạng sang quyền lực', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/TOYOTA/TOYOTA_Toyota_Land_Cruiser/64e8da71-b4fb-463c-b008-a84cb2a14e58.jpg', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-11T23:59:46.673' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (126, N'Toyota Hilux', N'Hilux', 2024, 1, CAST(668000000.00 AS Decimal(18, 2)), N'Bán tải bền bỉ, nồi đồng cối đá', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/TOYOTA/TOYOTA_Toyota_Hilux/f39b69ee-e4fc-442d-a09f-fe9bb31da6fd.jpg', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:00:44.450' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'Bán tải', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (127, N'Toyota Wigo', N'Wigo', 2024, 1, CAST(405000000.00 AS Decimal(18, 2)), N'Hatchback cỡ A nhập khẩu', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/TOYOTA/TOYOTA_Toyota_Wigo/09d90038-5107-4a0c-9ce6-708904fb9408.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:01:08.143' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Hatchback', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (128, N'Toyota Avanza Premio', N'Avanza', 2024, 1, CAST(598000000.00 AS Decimal(18, 2)), N'MPV 7 chỗ giá rẻ thực dụng', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/TOYOTA/TOYOTA_Toyota_Avanza_Premio/b61a9974-e27c-40f6-b07a-929dba62ab59.jpg', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:01:53.847' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (129, N'Toyota Alphard', N'Alphard', 2024, 1, CAST(4470000000.00 AS Decimal(18, 2)), N'Chuyên cơ mặt đất hạng sang', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/TOYOTA/TOYOTA_Toyota_Alphard/14131834-4d1c-49d9-8df1-f2b018c89563.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:02:24.300' AS DateTime), N'Hybrid', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (130, N'Toyota Land Cruiser Prado', N'Prado', 2024, 1, CAST(2628000000.00 AS Decimal(18, 2)), N'SUV việt dã cao cấp', N'TOYOTA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/TOYOTA/TOYOTA_Toyota_Land_Cruiser_Prado/45f241ae-6196-4fd7-b48c-38a249ebfa3f.jpg', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:02:56.170' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (131, N'Hyundai Grand i10', N'Grand i10', 2024, 1, CAST(435000000.00 AS Decimal(18, 2)), N'Hatchback quốc dân chạy dịch vụ', N'HYUNDAI', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/HYUNDAI/HYUNDAI_Hyundai_Grand_i10/49e60714-bfb5-4aa7-948f-55cb8b0a2103.jpg', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:03:57.030' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Hatchback', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (132, N'Hyundai Creta', N'Creta', 2024, 1, CAST(650000000.00 AS Decimal(18, 2)), N'Tiểu Tucson thiết kế thể thao', N'HYUNDAI', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/HYUNDAI/HYUNDAI_Hyundai_Creta/993b1ff0-0b49-4047-8f78-5b2b3af40422.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:04:28.467' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (133, N'Hyundai Stargazer', N'Stargazer', 2024, 1, CAST(575000000.00 AS Decimal(18, 2)), N'MPV 7 chỗ thiết kế phi thuyền', N'HYUNDAI', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/HYUNDAI/HYUNDAI_Hyundai_Stargazer/f8121d4f-7af9-4383-bcfb-a3c93af67242.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:05:05.027' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (134, N'Hyundai Custin', N'Custin', 2024, 1, CAST(850000000.00 AS Decimal(18, 2)), N'MPV cỡ trung thiết kế trượt', N'HYUNDAI', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/HYUNDAI/HYUNDAI_Hyundai_Custin/e71187e9-9d4e-4723-a60b-470005580e9f.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:05:43.093' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (135, N'Hyundai Palisade', N'Palisade', 2024, 1, CAST(1469000000.00 AS Decimal(18, 2)), N'SUV cỡ lớn sang trọng', N'HYUNDAI', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/HYUNDAI/HYUNDAI_Hyundai_Palisade/3ad6bc40-1d99-45c3-aa2a-f271dc50c491.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:06:12.127' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (136, N'Hyundai Elantra', N'Elantra', 2024, 1, CAST(599000000.00 AS Decimal(18, 2)), N'Sedan hạng C cá tính', N'HYUNDAI', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/HYUNDAI/HYUNDAI_Hyundai_Elantra/d286c740-ff27-4c98-b7ee-72b789b03539.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:07:40.890' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (137, N'Hyundai Venue', N'Venue', 2024, 1, CAST(539000000.00 AS Decimal(18, 2)), N'SUV cỡ A nhỏ gọn linh hoạt', N'HYUNDAI', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/HYUNDAI/HYUNDAI_Hyundai_Venue/ee2b3f7c-fb3d-4dfa-90d5-594286310e70.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:07:34.303' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (138, N'Hyundai Staria', N'Staria', 2024, 1, CAST(1500000000.00 AS Decimal(18, 2)), N'MPV cỡ lớn VIP', N'HYUNDAI', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/HYUNDAI/HYUNDAI_Hyundai_Staria/8b3d11bd-329b-4b35-b362-fd37be0e9263.jpg', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:09:19.443' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (139, N'Hyundai Ioniq 5', N'Ioniq 5', 2024, 1, CAST(1300000000.00 AS Decimal(18, 2)), N'Xe điện thuần túy tương lai', N'HYUNDAI', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/HYUNDAI/HYUNDAI_Hyundai_Ioniq_5/021f2f47-5cfe-44df-be1d-2c5d73c2b867.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:09:46.463' AS DateTime), N'Điện', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (140, N'Hyundai Kona', N'Kona', 2024, 1, CAST(636000000.00 AS Decimal(18, 2)), N'CUV cỡ B mạnh mẽ', N'HYUNDAI', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/HYUNDAI/HYUNDAI_Hyundai_Kona/7b10c2f5-f17b-4ab4-b502-8cff5baaf548.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:10:15.490' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (141, N'Kia Morning', N'Morning', 2024, 1, CAST(424000000.00 AS Decimal(18, 2)), N'Hatchback nhỏ gọn đi phố', N'KIA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/KIA/KIA_Kia_Morning/f15d7fe9-0b69-4fa7-9f65-fd59e3fc6136.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:11:37.247' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Hatchback', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (142, N'Kia Sonet', N'Sonet', 2024, 1, CAST(574000000.00 AS Decimal(18, 2)), N'SUV cỡ A bán chạy nhất', N'KIA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/KIA/KIA_Kia_Sonet/a71263b9-9838-47aa-bdd9-e4268f390cdd.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:12:01.757' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (143, N'Kia Carens', N'Carens', 2024, 1, CAST(629000000.00 AS Decimal(18, 2)), N'MPV gia đình thiết kế SUV', N'KIA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/KIA/KIA_Kia_Carens/9330aadf-00e9-4ff5-9c97-82cedeff5bad.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:12:30.007' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (144, N'Kia Sportage', N'Sportage', 2024, 1, CAST(799000000.00 AS Decimal(18, 2)), N'CUV cỡ C thiết kế phá cách', N'KIA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/KIA/KIA_Kia_Sportage/aec280ad-63bc-403d-b327-4122c0823f42.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:13:09.833' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (145, N'Kia Sorento', N'Sorento', 2024, 1, CAST(964000000.00 AS Decimal(18, 2)), N'SUV cỡ D nhiều công nghệ', N'KIA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/KIA/KIA_Kia_Sorento/b02ac034-be41-4c20-98b9-a0e29c14badf.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:13:35.637' AS DateTime), N'Hybrid', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (146, N'Kia K5', N'K5', 2024, 1, CAST(859000000.00 AS Decimal(18, 2)), N'Sedan hạng D dáng Fastback', N'KIA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/KIA/KIA_Kia_K5/accf7420-6799-4baa-af70-483304aec8a6.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:14:08.763' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (147, N'Kia Telluride', N'Telluride', 2024, 1, CAST(2000000000.00 AS Decimal(18, 2)), N'SUV cỡ lớn full-size', N'KIA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/KIA/KIA_Kia_Telluride/817d91ef-6cf6-40ba-b87c-5ef05c990c49.jpg', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:14:41.377' AS DateTime), N'Xăng', 0, CAST(N'2026-05-12T00:14:27.363' AS DateTime), 1, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (148, N'Kia Soluto', N'Soluto', 2024, 1, CAST(414000000.00 AS Decimal(18, 2)), N'Sedan hạng B giá rẻ', N'KIA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/KIA/KIA_Kia_Soluto/2a046100-f096-4f13-8347-0075236d2dcf.png', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:15:15.307' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (149, N'Kia Rondo', N'Rondo', 2024, 1, CAST(559000000.00 AS Decimal(18, 2)), N'MPV 7 chỗ đô thị nhỏ gọn', N'KIA', CAST(0.00 AS Decimal(18, 2)), N'/uploads/Cars/KIA/KIA_Kia_Rondo/dab5ef2c-ec04-4b05-bca9-025c477f79dd.jpg', 2, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T00:15:43.083' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (150, N'Kia Rio', N'Rio', 2024, 1, CAST(470000000.00 AS Decimal(18, 2)), N'Hatchback hạng B thể thao', N'Kia', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Hatchback', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (151, N'Mazda 2', N'Mazda 2', 2024, 1, CAST(415000000.00 AS Decimal(18, 2)), N'Sedan/Hatchback cỡ B thời trang', N'Mazda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (152, N'Mazda 6', N'Mazda 6', 2024, 1, CAST(769000000.00 AS Decimal(18, 2)), N'Sedan hạng D lịch lãm', N'Mazda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (153, N'Mazda CX-3', N'CX-3', 2024, 1, CAST(524000000.00 AS Decimal(18, 2)), N'CUV cỡ B linh hoạt', N'Mazda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (154, N'Mazda CX-30', N'CX-30', 2024, 1, CAST(699000000.00 AS Decimal(18, 2)), N'CUV cỡ B+ thiết kế đẹp mắt', N'Mazda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (155, N'Mazda BT-50', N'BT-50', 2024, 1, CAST(584000000.00 AS Decimal(18, 2)), N'Bán tải dùng chung khung gầm D-Max', N'Mazda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'Bán tải', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (156, N'Mazda CX-50', N'CX-50', 2024, 1, CAST(1200000000.00 AS Decimal(18, 2)), N'SUV việt dã phong cách mới', N'Mazda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (157, N'Mazda CX-70', N'CX-70', 2024, 1, CAST(1500000000.00 AS Decimal(18, 2)), N'CUV cỡ trung thể thao cao cấp', N'Mazda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Hybrid', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (158, N'Mazda CX-90', N'CX-90', 2024, 1, CAST(2000000000.00 AS Decimal(18, 2)), N'SUV flagship thay thế CX-9', N'Mazda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Hybrid', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (159, N'Mazda MX-5', N'MX-5 Miata', 2024, 1, CAST(2000000000.00 AS Decimal(18, 2)), N'Xe thể thao mui trần biểu tượng', N'Mazda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Coupe', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (160, N'Mazda CX-4', N'CX-4', 2024, 1, CAST(800000000.00 AS Decimal(18, 2)), N'CUV lai Coupe cá tính', N'Mazda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (161, N'Mitsubishi Attrage', N'Attrage', 2024, 1, CAST(380000000.00 AS Decimal(18, 2)), N'Sedan cỡ B siêu tiết kiệm xăng', N'Mitsubishi', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (162, N'Mitsubishi Outlander', N'Outlander', 2024, 1, CAST(825000000.00 AS Decimal(18, 2)), N'CUV 5+2 gia đình cách âm tốt', N'Mitsubishi', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (163, N'Mitsubishi Triton', N'Triton', 2024, 1, CAST(650000000.00 AS Decimal(18, 2)), N'Bán tải mạnh mẽ, linh hoạt', N'Mitsubishi', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'Bán tải', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (164, N'Mitsubishi Pajero Sport', N'Pajero Sport', 2024, 1, CAST(1130000000.00 AS Decimal(18, 2)), N'SUV 7 chỗ việt dã dẫn động Super Select', N'Mitsubishi', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (165, N'Mitsubishi Xpander Cross', N'Xpander Cross', 2024, 1, CAST(698000000.00 AS Decimal(18, 2)), N'MPV phong cách SUV thể thao', N'Mitsubishi', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (166, N'Mitsubishi Mirage', N'Mirage', 2024, 1, CAST(350000000.00 AS Decimal(18, 2)), N'Hatchback nhỏ gọn đi phố', N'Mitsubishi', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Hatchback', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (167, N'Mitsubishi Grandis', N'Grandis', 2011, 2, CAST(300000000.00 AS Decimal(18, 2)), N'MPV huyền thoại vang bóng một thời', N'Mitsubishi', CAST(50000.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (168, N'Mitsubishi Zinger', N'Zinger', 2010, 2, CAST(200000000.00 AS Decimal(18, 2)), N'MPV thực dụng khung gầm SUV', N'Mitsubishi', CAST(80000.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số sàn', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (169, N'Mitsubishi Lancer', N'Lancer', 2015, 2, CAST(400000000.00 AS Decimal(18, 2)), N'Sedan thể thao đậm chất rally', N'Mitsubishi', CAST(40000.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (170, N'Mitsubishi Eclipse Cross', N'Eclipse Cross', 2024, 1, CAST(900000000.00 AS Decimal(18, 2)), N'CUV góc cạnh thể thao', N'Mitsubishi', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (171, N'Ford Ranger Raptor', N'Raptor', 2024, 1, CAST(1299000000.00 AS Decimal(18, 2)), N'Siêu bán tải hiệu năng cao', N'Ford', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số tự động', N'Bán tải', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (172, N'Ford Explorer', N'Explorer', 2024, 1, CAST(2399000000.00 AS Decimal(18, 2)), N'SUV 7 chỗ full-size cỡ lớn Mỹ', N'Ford', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (173, N'Ford Transit', N'Transit', 2024, 1, CAST(849000000.00 AS Decimal(18, 2)), N'Xe khách 16 chỗ chạy dịch vụ', N'Ford', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Dầu', 0, NULL, NULL, N'Số sàn', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (174, N'Ford EcoSport', N'EcoSport', 2021, 2, CAST(450000000.00 AS Decimal(18, 2)), N'SUV đô thị từng làm mưa làm gió', N'Ford', CAST(30000.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (175, N'Ford Escape', N'Escape', 2024, 1, CAST(1100000000.00 AS Decimal(18, 2)), N'CUV cỡ C thiết kế mềm mại', N'Ford', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (176, N'Ford Bronco', N'Bronco', 2024, 1, CAST(3000000000.00 AS Decimal(18, 2)), N'SUV off-road đối thủ Wrangler', N'Ford', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (177, N'Ford Mustang', N'Mustang', 2024, 1, CAST(3500000000.00 AS Decimal(18, 2)), N'Xe cơ bắp thể thao huyền thoại', N'Ford', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Coupe', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (178, N'Ford F-150', N'F-150', 2024, 1, CAST(4000000000.00 AS Decimal(18, 2)), N'Bán tải cỡ lớn Vua đường phố', N'Ford', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Bán tải', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (179, N'Ford Puma', N'Puma', 2024, 1, CAST(750000000.00 AS Decimal(18, 2)), N'CUV cỡ B thiết kế thể thao', N'Ford', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (180, N'Ford Edge', N'Edge', 2024, 1, CAST(1800000000.00 AS Decimal(18, 2)), N'SUV cỡ trung cao cấp', N'Ford', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (181, N'Honda HR-V', N'HR-V', 2024, 1, CAST(699000000.00 AS Decimal(18, 2)), N'CUV cỡ B thiết kế lai Coupe', N'Honda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (182, N'Honda BR-V', N'BR-V', 2024, 1, CAST(661000000.00 AS Decimal(18, 2)), N'MPV 7 chỗ an toàn Sensing', N'Honda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (183, N'Honda Accord', N'Accord', 2024, 1, CAST(1319000000.00 AS Decimal(18, 2)), N'Sedan hạng D doanh nhân cảm giác lái bốc', N'Honda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (184, N'Honda Brio', N'Brio', 2022, 2, CAST(380000000.00 AS Decimal(18, 2)), N'Hatchback cỡ A thể thao cá tính', N'Honda', CAST(15000.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Hatchback', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (185, N'Honda Jazz', N'Jazz', 2020, 2, CAST(450000000.00 AS Decimal(18, 2)), N'Hatchback cỡ B gập ghế Magic Seat', N'Honda', CAST(40000.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Hatchback', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (186, N'Honda Pilot', N'Pilot', 2024, 1, CAST(3500000000.00 AS Decimal(18, 2)), N'SUV cỡ lớn 3 hàng ghế nhập Mỹ', N'Honda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (187, N'Honda Odyssey', N'Odyssey', 2024, 1, CAST(2500000000.00 AS Decimal(18, 2)), N'Minivan gia đình cao cấp rộng rãi', N'Honda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'MPV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (188, N'Honda Ridgeline', N'Ridgeline', 2024, 1, CAST(3000000000.00 AS Decimal(18, 2)), N'Bán tải Unibody êm ái như SUV', N'Honda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Bán tải', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (189, N'Honda Passport', N'Passport', 2024, 1, CAST(2800000000.00 AS Decimal(18, 2)), N'SUV việt dã nam tính gầm cao', N'Honda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (190, N'Honda WR-V', N'WR-V', 2024, 1, CAST(550000000.00 AS Decimal(18, 2)), N'CUV cỡ A siêu linh hoạt trong phố', N'Honda', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (191, N'BMW 320i Sport Line', N'3 Series', 2024, 1, CAST(1399000000.00 AS Decimal(18, 2)), N'Sedan thể thao sang trọng', N'BMW', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (192, N'BMW X5 xDrive40i', N'X5', 2024, 1, CAST(4019000000.00 AS Decimal(18, 2)), N'SUV thể thao hạng sang cỡ trung', N'BMW', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (193, N'Mercedes-Benz C200 Avantgarde', N'C-Class', 2024, 1, CAST(1599000000.00 AS Decimal(18, 2)), N'Sedan hạng sang bán chạy nhất', N'Mercedes', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Hybrid', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (194, N'Mercedes-Benz GLC 300 4MATIC', N'GLC', 2024, 1, CAST(2799000000.00 AS Decimal(18, 2)), N'SUV hạng sang thống trị phân khúc', N'Mercedes', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (195, N'Audi Q5 S-line', N'Q5', 2024, 1, CAST(2400000000.00 AS Decimal(18, 2)), N'SUV Quattro sang trọng êm ái', N'Audi', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (196, N'Lexus RX 350', N'RX', 2024, 1, CAST(3430000000.00 AS Decimal(18, 2)), N'SUV Nhật Bản êm ái, giữ giá số 1', N'Lexus', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (197, N'Porsche Macan', N'Macan', 2024, 1, CAST(3350000000.00 AS Decimal(18, 2)), N'CUV thể thao hạng sang hiệu năng cao', N'Porsche', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (198, N'Volvo XC60 Ultimate', N'XC60', 2024, 1, CAST(2320000000.00 AS Decimal(18, 2)), N'SUV an toàn nhất thế giới đến từ Thụy Điển', N'Volvo', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Hybrid', 0, NULL, NULL, N'Số tự động', N'SUV', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (199, N'Peugeot 2008', N'2008', 2024, 1, CAST(709000000.00 AS Decimal(18, 2)), N'CUV cỡ B thiết kế nanh sư tử châu Âu', N'Peugeot', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Crossover', NULL, NULL)
GO
INSERT [dbo].[Cars] ([CarId], [Name], [Model], [Year], [Condition], [Price], [Description], [Brand], [Mileage], [ImageUrl], [Status], [CreatedAt], [UpdatedAt], [FuelType], [IsDeleted], [DeletedAt], [DeletedBy], [Transmission], [BodyStyle], [RejectionReason], [CreatedByUserId]) VALUES (200, N'MG5', N'MG5', 2024, 1, CAST(399000000.00 AS Decimal(18, 2)), N'Sedan cỡ C giá sốc thiết kế Coupe', N'MG', CAST(0.00 AS Decimal(18, 2)), NULL, 1, CAST(N'2026-05-12T02:32:26.760' AS DateTime), CAST(N'2026-05-12T02:32:26.760' AS DateTime), N'Xăng', 0, NULL, NULL, N'Số tự động', N'Sedan', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Cars] OFF
GO
SET IDENTITY_INSERT [dbo].[CarSpecifications] ON 
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (63, 10, N'Nội thất', N'Số chỗ ngồi', N'7')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (105, 5, N'Nội thất', N'Số chỗ ngồi', N'5')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (106, 6, N'Nội thất', N'Số chỗ ngồi', N'7')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (107, 7, N'Nội thất', N'Số chỗ ngồi', N'5')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (108, 7, N'Vận hành', N'Hộp số', N'Tự động')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (109, 8, N'Nội thất', N'Số chỗ ngồi', N'5')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (110, 8, N'Động cơ', N'Loại nhiên liệu', N'Xăng')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (111, 9, N'Nội thất', N'Số chỗ ngồi', N'7')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (112, 9, N'Vận hành', N'Hệ dẫn động', N'4x2/4x4 (tùy phiên bản)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (289, 11, N'Kích thước', N'Dài x Rộng x Cao', N'5.118 x 2.254 x 1.696 mm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (290, 11, N'Kích thước', N'Chiều dài cơ sở', N'3.150 mm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (291, 11, N'Kích thước', N'Khoảng sáng gầm', N'189 mm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (292, 11, N'Động cơ', N'Loại động cơ', N'2 Motor điện (AWD)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (293, 11, N'Động cơ', N'Công suất tối đa', N'300 kW (402 HP)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (294, 11, N'Động cơ', N'Mô-men xoắn cực đại', N'620 Nm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (295, 11, N'Pin & Sạc', N'Quãng đường di chuyển (WLTP)', N'~423 km (Pin tiêu chuẩn)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (296, 11, N'Pin & Sạc', N'Thời gian sạc nhanh (10-70%)', N'~26 phút')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (297, 11, N'Ngoại thất', N'Đèn pha', N'LED Matrix')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (298, 11, N'Ngoại thất', N'Kích thước La-zăng', N'21 inch')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (299, 11, N'Nội thất', N'Màn hình giải trí', N'Cảm ứng 15.6 inch')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (300, 11, N'Nội thất', N'Số chỗ ngồi', N'7 chỗ (Option 6 chỗ ghế cơ trưởng)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (301, 14, N'Động cơ & Vận hành', N'Kiểu động cơ', N'1.5L DOHC VTEC TURBO, 4 xi-lanh thẳng hàng, 16 van')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (302, 14, N'Động cơ & Vận hành', N'Dung tích xi-lanh', N'1.498 cc')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (303, 14, N'Động cơ & Vận hành', N'Công suất cực đại', N'188 Hp @ 6.000 rpm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (304, 14, N'Động cơ & Vận hành', N'Mô-men xoắn cực đại', N'240 Nm @ 1.700 - 5.000 rpm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (305, 14, N'Động cơ & Vận hành', N'Hộp số', N'Vô cấp (CVT), ứng dụng G-Design Shift')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (306, 14, N'Động cơ & Vận hành', N'Hệ dẫn động', N'Cầu trước (FWD) hoặc Hai cầu chủ động (AWD)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (307, 14, N'Động cơ & Vận hành', N'Dung tích thùng nhiên liệu', N'57 lít')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (308, 14, N'Kích thước & Trọng lượng', N'Kích thước tổng thể (DxRxC)', N'4.691 x 1.866 x 1.681 (mm)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (309, 14, N'Kích thước & Trọng lượng', N'Chiều dài cơ sở', N'2.701 mm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (310, 14, N'Kích thước & Trọng lượng', N'Chiều rộng cơ sở (Trước/Sau)', N'1.611 / 1.627 (mm)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (311, 14, N'Kích thước & Trọng lượng', N'Khoảng sáng gầm xe', N'198 mm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (312, 14, N'Kích thước & Trọng lượng', N'Bán kính vòng quay tối thiểu', N'5,5 m')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (313, 14, N'Kích thước & Trọng lượng', N'Cỡ lốp', N'235/60R18')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (314, 14, N'An toàn & Hỗ trợ lái', N'Gói an toàn chủ động', N'Honda SENSING (CMBS, AHB, ACC với LSF, RDM, LKAS, LCDN)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (315, 14, N'An toàn & Hỗ trợ lái', N'Hệ thống túi khí', N'8 túi khí (Hàng ghế trước, bên hông, rèm và đầu gối)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (316, 14, N'An toàn & Hỗ trợ lái', N'Camera quan sát làn đường', N'Honda LaneWatch')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (317, 14, N'An toàn & Hỗ trợ lái', N'Hệ thống camera hỗ trợ', N'Camera lùi 3 góc quay (bản L) hoặc Camera 360 độ (bản L AWD)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (318, 14, N'An toàn & Hỗ trợ lái', N'Cảm biến đỗ xe', N'Cảm biến trước và sau')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (319, 14, N'An toàn & Hỗ trợ lái', N'Hỗ trợ đổ đèo', N'Có (HDC)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (320, 14, N'An toàn & Hỗ trợ lái', N'Cảnh báo chống buồn ngủ', N'Có (Driver Attention Monitor)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (321, 14, N'Nội thất & Tiện nghi', N'Số chỗ ngồi', N'7 chỗ (Cấu hình 5+2)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (322, 14, N'Nội thất & Tiện nghi', N'Chất liệu ghế', N'Da cao cấp (Màu Đen)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (323, 14, N'Nội thất & Tiện nghi', N'Ghế lái', N'Chỉnh điện 8 hướng, nhớ 2 vị trí')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (324, 14, N'Nội thất & Tiện nghi', N'Ghế phụ', N'Chỉnh điện 4 hướng')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (325, 14, N'Nội thất & Tiện nghi', N'Bảng đồng hồ trung tâm', N'Màn hình Digital 10.2 inch')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (326, 14, N'Nội thất & Tiện nghi', N'Màn hình giải trí', N'Cảm ứng 9 inch, kết nối Apple CarPlay không dây / Android Auto')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (327, 14, N'Nội thất & Tiện nghi', N'Hệ thống âm thanh', N'8 loa chất lượng cao')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (328, 14, N'Nội thất & Tiện nghi', N'Hệ thống điều hòa', N'Tự động 2 vùng độc lập, có cửa gió hàng ghế 2 và hàng ghế 3')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (329, 14, N'Nội thất & Tiện nghi', N'Sạc không dây', N'Có')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (330, 14, N'Nội thất & Tiện nghi', N'Phanh tay điện tử', N'Tích hợp giữ phanh tự động (Brake Hold)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (361, 16, N'Động cơ & Vận hành', N'Kiểu động cơ', N'1 động cơ điện (Base) hoặc Động cơ điện kép (Plus)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (362, 16, N'Động cơ & Vận hành', N'Công suất cực đại', N'174 Hp (Base) hoặc 349 Hp (Plus)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (363, 16, N'Động cơ & Vận hành', N'Mô-men xoắn cực đại', N'250 Nm (Base) hoặc 500 Nm (Plus)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (364, 16, N'Động cơ & Vận hành', N'Hệ dẫn động', N'Cầu trước FWD (Base) hoặc Hai cầu AWD (Plus)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (365, 16, N'Động cơ & Vận hành', N'Dung lượng pin khả dụng', N'59.6 kWh (Base) hoặc 75.3 kWh (Plus)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (366, 16, N'Động cơ & Vận hành', N'Quãng đường di chuyển (WLTP)', N'Lên tới 375 km (Base) hoặc 431 km (Plus) / một lần sạc đầy')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (367, 16, N'Kích thước & Trọng lượng', N'Kích thước tổng thể (DxRxC)', N'4.545 x 1.890 x 1.635 (mm)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (368, 16, N'Kích thước & Trọng lượng', N'Chiều dài cơ sở', N'2.840 mm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (369, 16, N'Kích thước & Trọng lượng', N'Khoảng sáng gầm xe', N'190 mm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (370, 16, N'Kích thước & Trọng lượng', N'Cỡ lốp', N'19 inch (Base) hoặc 20 inch / 21 inch (Plus)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (371, 16, N'Nội thất & Tiện nghi', N'Số chỗ ngồi', N'5 chỗ')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (372, 16, N'Nội thất & Tiện nghi', N'Màn hình giải trí trung tâm', N'Màn hình cảm ứng 12.9 inch xoay nhẹ hướng về người lái')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (373, 16, N'Nội thất & Tiện nghi', N'Hệ thống hiển thị thông tin', N'Màn hình HUD hiển thị kính lái')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (374, 16, N'Nội thất & Tiện nghi', N'Trần kính toàn cảnh', N'Trần kính Panorama cao cấp (Tùy chọn trên bản Plus)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (375, 16, N'Nội thất & Tiện nghi', N'Trợ lý ảo', N'Trợ lý ảo VinFast điều khiển giọng nói tiếng Việt đa vùng miền')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (376, 2, N'Nội thất', N'Số chỗ ngồi', N'4')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (377, 2, N'Pin', N'Quãng đường di chuyển', N'210 km')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (378, 3, N'Nội thất', N'Số chỗ ngồi', N'5')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (379, 4, N'Nội thất', N'Số chỗ ngồi', N'5')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (380, 4, N'Động cơ', N'Loại nhiên liệu', N'Điện')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (381, 15, N'Động cơ & Vận hành', N'Kiểu động cơ', N'Hai động cơ điện (Dual Motor) đặt ở cầu trước và sau')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (382, 15, N'Động cơ & Vận hành', N'Công suất cực đại', N'349 Hp (Eco) hoặc 402 Hp (Plus)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (383, 15, N'Động cơ & Vận hành', N'Mô-men xoắn cực đại', N'500 Nm (Eco) hoặc 620 Nm (Plus)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (384, 15, N'Động cơ & Vận hành', N'Khả năng tăng tốc (0-100 km/h)', N'5.9 giây (Eco) hoặc 5.5 giây (Plus)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (385, 15, N'Động cơ & Vận hành', N'Hệ dẫn động', N'Hai cầu toàn thời gian (AWD)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (386, 15, N'Động cơ & Vận hành', N'Quãng đường di chuyển (WLTP)', N'Khoảng 400 - 425 km / một lần sạc đầy')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (387, 15, N'Kích thước & Trọng lượng', N'Kích thước tổng thể (DxRxC)', N'4.750 x 1.934 x 1.667 (mm)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (388, 15, N'Kích thước & Trọng lượng', N'Chiều dài cơ sở', N'2.950 mm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (389, 15, N'Kích thước & Trọng lượng', N'Khoảng sáng gầm xe', N'175 mm')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (390, 15, N'Kích thước & Trọng lượng', N'Cỡ lốp', N'19 inch (Eco) hoặc 20 inch (Plus)')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (391, 15, N'Nội thất & Tiện nghi', N'Số chỗ ngồi', N'5 chỗ')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (392, 15, N'Nội thất & Tiện nghi', N'Màn hình giải trí trung tâm', N'Màn hình cảm ứng 15.6 inch siêu lớn')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (393, 15, N'Nội thất & Tiện nghi', N'Hệ thống hiển thị thông tin', N'Màn hình HUD (hiển thị trên kính lái) thay thế đồng hồ truyền thống')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (394, 15, N'Nội thất & Tiện nghi', N'Hệ thống điều hòa', N'Tự động 2 vùng độc lập, có màng lọc than hoạt tính & kiểm soát chất lượng không khí')
GO
INSERT [dbo].[CarSpecifications] ([SpecId], [CarId], [Category], [SpecName], [SpecValue]) VALUES (395, 15, N'Nội thất & Tiện nghi', N'Trợ lý ảo', N'Trợ lý ảo VinFast (điều khiển giọng nói tiếng Việt đa vùng miền)')
GO
SET IDENTITY_INSERT [dbo].[CarSpecifications] OFF
GO
SET IDENTITY_INSERT [dbo].[Consignments] ON 
GO
INSERT [dbo].[Consignments] ([ConsignmentId], [Brand], [Model], [Year], [Mileage], [ConditionDescription], [ExpectedPrice], [AgreedPrice], [CommissionRate], [Status], [LinkedCarId], [CreatedAt], [UpdatedAt], [GuestName], [GuestPhone], [GuestEmail]) VALUES (1, N'Vinfast', N'vf8', 2022, CAST(16000.00 AS Decimal(18, 2)), NULL, CAST(800000000.00 AS Decimal(18, 2)), NULL, NULL, N'Completed', NULL, CAST(N'2026-05-02T07:48:34.8797483' AS DateTime2), CAST(N'2026-05-02T08:45:37.1228093' AS DateTime2), N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com')
GO
INSERT [dbo].[Consignments] ([ConsignmentId], [Brand], [Model], [Year], [Mileage], [ConditionDescription], [ExpectedPrice], [AgreedPrice], [CommissionRate], [Status], [LinkedCarId], [CreatedAt], [UpdatedAt], [GuestName], [GuestPhone], [GuestEmail]) VALUES (2, N'Vinfast', N'vf9', 2022, CAST(16000.00 AS Decimal(18, 2)), NULL, CAST(800000000.00 AS Decimal(18, 2)), NULL, NULL, N'Approved', NULL, CAST(N'2026-05-02T11:08:12.1565381' AS DateTime2), CAST(N'2026-05-02T12:02:44.4418491' AS DateTime2), N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com')
GO
INSERT [dbo].[Consignments] ([ConsignmentId], [Brand], [Model], [Year], [Mileage], [ConditionDescription], [ExpectedPrice], [AgreedPrice], [CommissionRate], [Status], [LinkedCarId], [CreatedAt], [UpdatedAt], [GuestName], [GuestPhone], [GuestEmail]) VALUES (3, N'Vinfast', N'vf8', 2022, CAST(16000.00 AS Decimal(18, 2)), N'mô tả tình trạng của xe ở đây', CAST(800000000.00 AS Decimal(18, 2)), NULL, NULL, N'Completed', NULL, CAST(N'2026-05-02T11:08:37.1854216' AS DateTime2), CAST(N'2026-05-02T12:02:36.9832604' AS DateTime2), N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com')
GO
INSERT [dbo].[Consignments] ([ConsignmentId], [Brand], [Model], [Year], [Mileage], [ConditionDescription], [ExpectedPrice], [AgreedPrice], [CommissionRate], [Status], [LinkedCarId], [CreatedAt], [UpdatedAt], [GuestName], [GuestPhone], [GuestEmail]) VALUES (4, N'Vinfast', N'vf10', 2022, CAST(16000.00 AS Decimal(18, 2)), NULL, CAST(800000000.00 AS Decimal(18, 2)), NULL, NULL, N'Approved', NULL, CAST(N'2026-05-02T11:19:55.0812825' AS DateTime2), CAST(N'2026-05-05T21:21:02.9936830' AS DateTime2), N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com')
GO
INSERT [dbo].[Consignments] ([ConsignmentId], [Brand], [Model], [Year], [Mileage], [ConditionDescription], [ExpectedPrice], [AgreedPrice], [CommissionRate], [Status], [LinkedCarId], [CreatedAt], [UpdatedAt], [GuestName], [GuestPhone], [GuestEmail]) VALUES (5, N'Vinfast', N'vf11', 2022, CAST(16000.00 AS Decimal(18, 2)), NULL, CAST(800000000.00 AS Decimal(18, 2)), NULL, NULL, N'Approved', NULL, CAST(N'2026-05-02T11:21:57.8259484' AS DateTime2), CAST(N'2026-05-02T12:40:41.4313828' AS DateTime2), N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com')
GO
INSERT [dbo].[Consignments] ([ConsignmentId], [Brand], [Model], [Year], [Mileage], [ConditionDescription], [ExpectedPrice], [AgreedPrice], [CommissionRate], [Status], [LinkedCarId], [CreatedAt], [UpdatedAt], [GuestName], [GuestPhone], [GuestEmail]) VALUES (6, N'TEST', N'VF3', 2025, CAST(55555.00 AS Decimal(18, 2)), N'ùyydgqưuigduiqưgudiogqư', CAST(400000000.00 AS Decimal(18, 2)), NULL, NULL, N'Pending', NULL, CAST(N'2026-05-07T14:09:08.0775591' AS DateTime2), CAST(N'2026-05-07T14:09:08.0775766' AS DateTime2), N'Nguyễn Hữu Thiện', N'0379748675', NULL)
GO
SET IDENTITY_INSERT [dbo].[Consignments] OFF
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
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (13, N'start', N'/uploads/Features/icon-start.PNG')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (14, N'Hệ thống trợ lái nâng cao (ADAS)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (15, N'Trợ lý ảo VinFast', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (16, N'Cửa sổ trời toàn cảnh (Panoramic Sunroof)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (17, N'Màn hình HUD (Hiển thị kính lái)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (18, N'Ghế massage/sưởi/thông gió', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (19, N'Camera 360 độ', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (20, N'Phanh giảm thiểu va chạm (CMBS)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (21, N'Đèn pha thích ứng tự động (AHB)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (22, N'Kiểm soát hành trình thích ứng bao gồm dải tốc độ thấp (ACC with LSF)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (23, N'Giảm thiểu chệch làn đường (RDM)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (24, N'Hỗ trợ giữ làn đường (LKAS)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (25, N'Thông báo xe phía trước khởi hành (LCDN)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (26, N'Camera quan sát làn đường (LaneWatch)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (27, N'Cảm biến đỗ xe trước/sau', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (28, N'Hệ thống cảnh báo chệch làn đường & Hỗ trợ đổ đèo (HDC)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (29, N'Cảnh báo chống buồn ngủ (Driver Attention Monitor)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (30, N'8 Túi khí', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (31, N'Màn hình giải trí cảm ứng 9 inch', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (32, N'Bảng đồng hồ kỹ thuật số 10.2 inch', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (33, N'Hệ thống âm thanh 12 loa BOSE', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (34, N'Màn hình hiển thị thông tin trên kính lái (HUD)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (35, N'Sạc điện thoại không dây', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (36, N'Kết nối viễn thông Honda CONNECT', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (37, N'Điều hòa tự động 2 vùng độc lập', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (38, N'Cửa sổ trời toàn cảnh Panorama', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (39, N'Đèn pha LED tự động bật/tắt & tự động điều chỉnh góc chiếu', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (40, N'Cốp điện rảnh tay (Đá cốp) & Tự động đóng khi bước ra xa (Walk-away Close)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (41, N'Khởi động từ xa bằng chìa khóa', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (42, N'Phanh tay điện tử (EPB) & Giữ phanh tự động (Brake Hold)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (43, N'Chế độ lái Sport / Normal / Econ', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (44, N'Hỗ trợ di chuyển khi ùn tắc giao thông', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (45, N'Hỗ trợ lái trên đường cao tốc', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (46, N'Cảnh báo chệch làn & Hỗ trợ giữ làn đường', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (47, N'Giám sát hành trình thích ứng (ACC)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (48, N'Nhận diện biển báo giao thông', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (49, N'Phanh tự động khẩn cấp (AEB)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (50, N'Hỗ trợ đỗ xe thông minh & Tự động đỗ xe', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (51, N'Kết nối mua sắm trực tuyến, trò chơi điện tử trên màn hình', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (52, N'Cập nhật phần mềm qua mạng (FOTA)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (53, N'11 Túi khí an toàn', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (54, N'Hệ thống hỗ trợ lái nâng cao (ADAS)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (55, N'Cảnh báo chệch làn đường & Hỗ trợ giữ làn', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (56, N'Kiểm soát hành trình thích ứng (ACC)', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (57, N'Cảnh báo va chạm phía trước & Phanh khẩn cấp tự động', N'/uploads/Features/default-feature.png')
GO
INSERT [dbo].[Features] ([FeatureId], [FeatureName], [Icon]) VALUES (58, N'Ứng dụng giải trí và tiện ích thông minh (Smart Services)', N'/uploads/Features/default-feature.png')
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
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (4, NULL, 1, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Lê Quang Vinh vừa được cấp tài khoản ShowroomManager tại chi nhánh chúng ta.', N'/admin/users', NULL, 1, CAST(N'2026-04-27T10:59:14.0301776' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (5, NULL, 1, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Le Quang Vinh vừa được cấp tài khoản ShowroomSales tại chi nhánh chúng ta.', N'/admin/users', NULL, 1, CAST(N'2026-04-27T11:00:03.9543184' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (6, NULL, 1, N'Có lịch hẹn xem xe mới! 📅', N'Khách hàng WwangVinh04 (3814846150) vừa đặt lịch xem mẫu TEST TEST 1 vào lúc 23:58 ngày 30/04/2026.', N'/admin/bookings', N'Manager,ShowroomSales', 1, CAST(N'2026-04-28T22:59:02.3143081' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (7, NULL, 1, N'Có lịch hẹn xem xe mới! 📅', N'Khách hàng WwangVinh04 (3814846150) vừa đặt lịch xem mẫu TEST TEST 1 vào lúc 01:06 ngày 30/04/2026.', N'/admin/bookings', N'Manager,ShowroomSales', 1, CAST(N'2026-04-28T23:06:45.4230390' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (8, 1, NULL, N'🔄 Banner đã được cập nhật', N'Banner "banner 4" vừa được thay đổi nội dung hoặc thời gian hiển thị.', N'/admin/banners', N'Admin,Marketing', 0, CAST(N'2026-04-28T23:07:09.7712086' AS DateTime2), N'BANNER_UPDATE')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (9, NULL, 1, N'Có lịch hẹn xem xe mới! 📅', N'Khách hàng WwangVinh04 (0965346160) vừa đặt lịch xem mẫu TEST TEST 1 vào lúc  ngày 28/04/2026.', N'/admin/bookings', N'Manager,ShowroomSales', 1, CAST(N'2026-04-28T23:27:42.8582081' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (10, NULL, 1, N'Có lịch hẹn lái thử mới! 📅', N'Khách WwangVinh04 (0965346160) vừa đặt lịch lái thử TEST TEST 1 lúc 12:08 ngày 30/04/2026.', N'/admin/bookings', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-04-29T07:11:30.5103533' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (11, NULL, 1, N'Xe cần kiểm tra kỹ thuật 🔧', N'Xe TEST 1 cần kiểm tra trước lịch lái thử của khách WwangVinh04 vào 30/04/2026 lúc 12:08.', N'/admin/bookings/4', N'Technician', 0, CAST(N'2026-04-29T07:12:07.3924964' AS DateTime2), N'TechCheck')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (12, NULL, 1, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Lee Quang Vinh (Technician) vừa gia nhập đại gia đình chi nhánh chúng ta.', N'/admin/users', NULL, 1, CAST(N'2026-04-29T07:44:50.8927037' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (13, NULL, 1, N'Xe đã sẵn sàng ✅', N'Xe TEST 1 đã qua kiểm tra kỹ thuật. Vui lòng xác nhận lịch lái thử với khách WwangVinh04.', N'/admin/bookings/4', N'Sales,ShowroomSales,Manager', 1, CAST(N'2026-04-29T07:55:05.6711983' AS DateTime2), N'TechCheck')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (14, NULL, 1, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Le Van Vinh (Technician) vừa gia nhập đại gia đình chi nhánh chúng ta.', N'/admin/users', NULL, 1, CAST(N'2026-04-29T11:02:01.1656901' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (15, NULL, 1, N'Có lịch hẹn lái thử mới! 📅', N'Khách WwangVinh04 (0965346160) vừa đặt lịch lái thử TEST TEST 1 lúc 16:43 ngày 30/04/2026.', N'/admin/bookings', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-04-29T11:43:48.7720319' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (16, NULL, 1, N'Xe cần kiểm tra kỹ thuật 🔧', N'Xe TEST 1 cần kiểm tra trước lịch lái thử của khách WwangVinh04 vào 30/04/2026 lúc 16:43.', N'/admin/bookings/5', N'Technician', 0, CAST(N'2026-04-29T11:44:27.3246511' AS DateTime2), N'TechCheck')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (17, NULL, 1, N'Xe đã sẵn sàng ✅', N'Xe TEST 1 đã qua kiểm tra kỹ thuật. Vui lòng xác nhận lịch lái thử với khách WwangVinh04.', N'/admin/bookings/5', N'Sales,ShowroomSales,Manager', 1, CAST(N'2026-04-29T11:50:52.1776923' AS DateTime2), N'TechCheck')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (18, NULL, 1, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Le Vann Vinh (Sales) vừa gia nhập đại gia đình chi nhánh chúng ta.', N'/admin/users', NULL, 1, CAST(N'2026-04-29T12:31:42.9091496' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (19, NULL, 1, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Le Van Vinhh (Content) vừa gia nhập chi nhánh.', N'/admin/users', NULL, 1, CAST(N'2026-04-29T12:58:50.7253075' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (20, NULL, 1, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Le Vann VInhh (Marketing) vừa gia nhập chi nhánh.', N'/admin/users', NULL, 1, CAST(N'2026-04-29T12:59:22.7670888' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (21, NULL, NULL, N'Tung mã khuyến mãi mới! 🎁', N'Mã SDFGHGF (giảm 10%) vừa được kích hoạt. Marketing lên Banner, Sales lấy mã chốt khách ngay!', N'/admin/promotions', N'Marketing,Sales,ShowroomSales', 1, CAST(N'2026-04-29T13:29:03.3267018' AS DateTime2), N'Promotion')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (22, NULL, NULL, N'📰 Bài viết mới vừa đăng', N'Bài viết: "safea" đã được xuất bản.', N'/news', N'Marketing', 0, CAST(N'2026-04-29T13:49:12.7266081' AS DateTime2), N'ARTICLE')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (23, NULL, NULL, N'📰 Bài viết mới vừa đăng', N'Bài viết: "ssfbge" đã được xuất bản.', N'/news', N'Marketing', 0, CAST(N'2026-04-29T13:52:25.6707764' AS DateTime2), N'ARTICLE')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (24, NULL, NULL, N'Tung mã khuyến mãi mới! 🎁', N'Mã SDAFDGSDFMG (giảm 15%) vừa được kích hoạt. Marketing lên Banner, Sales lấy mã chốt khách ngay!', N'/admin/promotions', N'Marketing,Sales,ShowroomSales', 1, CAST(N'2026-04-29T13:55:03.1865465' AS DateTime2), N'Promotion')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (25, NULL, 1, N'Có lịch hẹn lái thử mới! 📅', N'Khách WwangVinh04 (0965346160) vừa đặt lịch lái thử VINFAST VinFast VF 9 Plus lúc 20:07 ngày 30/04/2026.', N'/admin/bookings', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-04-29T15:07:27.8578163' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (26, NULL, 1, N'Xe cần kiểm tra kỹ thuật 🔧', N'Xe VinFast VF 9 Plus cần kiểm tra trước lịch lái thử của khách WwangVinh04 vào 30/04/2026 lúc 20:07.', N'/admin/bookings/6', N'Technician', 0, CAST(N'2026-04-29T15:25:04.1149780' AS DateTime2), N'TechCheck')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (27, NULL, 1, N'Xe đã sẵn sàng ✅', N'Xe VinFast VF 9 Plus đã qua kiểm tra kỹ thuật. Vui lòng xác nhận lịch lái thử với khách WwangVinh04.', N'/admin/bookings/6', N'Sales,ShowroomSales,Manager', 1, CAST(N'2026-04-29T15:25:41.4058571' AS DateTime2), N'TechCheck')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (28, NULL, 1, N'Có lịch hẹn lái thử mới! 📅', N'Khách WwangVinh04 (0965346160) vừa đặt lịch lái thử VINFAST VinFast VF 9 Plus lúc 00:12 ngày 30/04/2026.', N'/admin/bookings', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-04-29T20:12:17.5177846' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (29, NULL, 1, N'Có lịch hẹn lái thử mới! 📅', N'Khách Lê Vinh (0965346160) vừa đặt lịch lái thử VINFAST VinFast VF 9 Plus lúc 01:33 ngày 14/05/2026.', N'/bookings', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-04-30T23:33:37.2271105' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (30, NULL, NULL, N'📰 Bài viết mới vừa đăng', N'Bài viết: "VinFast VF9 Plus: Trải Nghiệm Tiện Nghi Đẳng Cấp Trên Mẫu SUV Điện Cao Cấp Nhất" đã được xuất bản.', N'/news', N'Marketing', 0, CAST(N'2026-05-01T16:12:48.8450032' AS DateTime2), N'ARTICLE')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (1030, NULL, 1, N'Có lịch hẹn lái thử mới! 📅', N'Khách WwangVinh04 (03814846150) vừa đặt lịch lái thử VINFAST VinFast VF 9 Plus lúc 23:30 ngày 01/05/2026.', N'/bookings', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-01T23:30:52.6709964' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (1031, NULL, 2, N'Có lịch hẹn lái thử mới! 📅', N'Khách WwangVinh04 (03814846150) vừa đặt lịch lái thử VINFAST VinFast VF 9 Plus lúc 23:39 ngày 01/05/2026.', N'/bookings', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-01T23:39:25.4476672' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2030, NULL, NULL, N'Yêu cầu ký gửi xe mới 🚗', N'Khách WwangVinh04 (3814846150) muốn ký gửi Vinfast vf8 (2022). Vào kiểm tra ngay!', N'/consignments', N'Manager', 0, CAST(N'2026-05-02T14:48:34.9936958' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2031, NULL, NULL, N'Yêu cầu ký gửi xe mới 🚗', N'Khách WwangVinh04 (3814846150) muốn ký gửi Vinfast vf9 (2022). Vào kiểm tra ngay!', N'/consignments', N'Manager', 0, CAST(N'2026-05-02T18:08:12.2578716' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2032, NULL, NULL, N'Yêu cầu ký gửi xe mới 🚗', N'Khách WwangVinh04 (3814846150) muốn ký gửi Vinfast vf8 (2022). Vào kiểm tra ngay!', N'/consignments', N'Manager', 1, CAST(N'2026-05-02T18:08:37.1920840' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2033, NULL, NULL, N'Yêu cầu ký gửi xe mới 🚗', N'Khách WwangVinh04 (3814846150) muốn ký gửi Vinfast vf10 (2022). Vào kiểm tra ngay!', N'/consignments', N'Manager', 0, CAST(N'2026-05-02T18:19:55.1597243' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2034, NULL, NULL, N'Yêu cầu ký gửi xe mới 🚗', N'Khách WwangVinh04 (3814846150) muốn ký gửi Vinfast vf11 (2022). Vào kiểm tra ngay!', N'/consignments', N'Manager', 0, CAST(N'2026-05-02T18:21:57.8327091' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2035, NULL, 3, N'Nhân sự mới gia nhập! 🎉', N'Chào mừng Lê Quang Vinhh (ShowroomSales) vừa gia nhập chi nhánh.', N'/admin/users', NULL, 0, CAST(N'2026-05-02T20:47:40.9412084' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2036, NULL, 3, N'Có xe mới cần duyệt', N'Nhân viên vừa đăng mẫu TEST test 2. Sếp vào duyệt nhé!', N'/admin/cars/approve/12', N'ShowroomManager', 0, CAST(N'2026-05-02T21:55:22.0860753' AS DateTime2), N'CarApproval')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2037, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-E777. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/4', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T03:05:00.2910145' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2038, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-3011. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/5', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T13:51:44.6800509' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2039, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-A7A3. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/6', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:32:16.3419980' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2040, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-3A09. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/7', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:32:20.0616394' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2041, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-7580. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/8', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:32:20.7929745' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2042, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-0549. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/9', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:32:20.9325673' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2043, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-02DC. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/10', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:32:21.1016574' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2044, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-D557. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/11', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:32:22.8404466' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2045, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-54A9. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/12', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:32:23.0203483' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2046, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-D958. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/13', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:32:25.2100867' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2047, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-B8FD. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/14', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:32:25.3748150' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2048, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-0B11. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/15', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:32:25.5280078' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2049, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-220D. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/16', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:32:25.6787457' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2050, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-F158. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/17', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:32:25.8070124' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2051, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-9207. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/18', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:32:31.7226414' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2052, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-0D8A. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/19', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-03T14:45:38.9473436' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2053, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-99AA. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/20', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:45:39.5153521' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2054, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-C461. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/21', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:45:39.6688549' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2055, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-E3F2. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/22', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:45:39.9706515' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2056, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-FB95. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/23', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:45:40.1295235' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2057, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-E277. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/24', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:45:40.2807372' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2058, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-BC44. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/25', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:45:40.4373745' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2059, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-5BB8. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/26', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:45:40.5854758' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2060, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-637E. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/27', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:45:40.7301546' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2061, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-E19C. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/28', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:45:40.8694021' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2062, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-C5F3. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/29', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:52:17.2479718' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2063, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-FAD8. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/30', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:53:13.3687086' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2064, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-BE60. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/31', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:54:50.9745562' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2065, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-73E0. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/32', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:58:20.5589320' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2066, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-4B06. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/33', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:58:23.9902929' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2067, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-1CE7. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/34', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T14:58:49.0410914' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2068, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-357B. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/35', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T15:06:57.3411617' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2069, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-4802. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/36', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T15:07:00.9981532' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2070, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-6C25. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/37', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T15:07:12.9200787' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2071, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-A8C3. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/38', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T15:07:22.7199300' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2072, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-B10A. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/39', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T15:10:04.2961785' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2073, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng wangvinh (0379748675) vừa đặt đơn hàng OTO-7C76. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/40', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T15:16:22.5244051' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2074, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-0771. Tổng tiền: 1,589,000,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/41', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T15:22:51.7742367' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2075, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-798F. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/42', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T15:26:29.1918931' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2076, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-E339. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/43', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T15:27:04.8629393' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2077, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-2A9F. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/44', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T15:27:59.8132899' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2078, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-672B. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/45', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T15:28:14.4886129' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2079, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-3072. Tổng tiền: 0đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/46', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T15:28:21.2490272' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2080, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-4530. Tổng tiền: 1,430,100,000đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/47', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T15:33:14.3108637' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2081, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-A200. Tổng tiền: 158,900đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/48', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T19:47:11.5673512' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2082, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-267D. Tổng tiền: 158,900đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/49', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T20:24:16.1879103' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2083, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Lê Quang Vinh (0904059986) vừa đặt đơn hàng OTO-4E31. Tổng tiền: 158,900đ. Anh em gọi chốt ngay!', N'/admin/orders/detail/50', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T21:22:59.1506286' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2084, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Lê Quang Vinh (0904059986) vừa đặt đơn hàng OTO-8DA3. Tổng tiền: 1,589,000,000đ.', N'/admin/orders/detail/51', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T22:36:59.9187609' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2085, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Lê Quang Vinh (0941756860) vừa đặt đơn hàng OTO-602A. Tổng tiền: 158,900đ.', N'/admin/orders/detail/52', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T22:45:33.5627066' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2086, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Lê Quang Vinh (0941756860) vừa đặt đơn hàng OTO-71C5. Tổng tiền: 158,900đ.', N'/admin/orders/detail/53', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-03T22:46:27.5557138' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2087, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-8856. Tổng tiền: 158,900đ.', N'/admin/orders/detail/54', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-04T01:28:11.3627462' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2088, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-B027. Tổng tiền: 158,900đ.', N'/admin/orders/detail/55', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-04T01:28:27.4423744' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2089, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-631A. Tổng tiền: 158,900đ.', N'/admin/orders/detail/56', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-04T01:40:58.6317990' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2090, NULL, NULL, N'Tiền về tài khoản! 💰', N'Đơn hàng OTO-631A vừa được ghi nhận thanh toán 15,890đ qua PayOS (QR Code). Trạng thái: Đã cọc.', N'/admin/orders/detail/56', N'Manager,Admin', 1, CAST(N'2026-05-04T02:28:40.3759745' AS DateTime2), N'Payment')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2091, NULL, NULL, N'Tiền về tài khoản! 💰', N'Đơn hàng OTO-A200 vừa được ghi nhận thanh toán 15,890đ qua PayOS (QR Code). Trạng thái: Đã cọc.', N'/admin/orders/detail/48', N'Manager,Admin', 1, CAST(N'2026-05-04T02:29:38.6916621' AS DateTime2), N'Payment')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2092, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (0965346160) vừa đặt đơn hàng OTO-348B. Tổng tiền: 158,900đ.', N'/admin/orders/detail/57', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-04T02:41:50.0188267' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2093, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (0965346160) vừa đặt đơn hàng OTO-6859. Tổng tiền: 158,900đ.', N'/admin/orders/detail/58', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-04T02:41:57.4715510' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2094, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (0965346160) vừa đặt đơn hàng OTO-A7D9. Tổng tiền: 158,900đ.', N'/admin/orders/detail/59', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-04T02:43:57.9685475' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2095, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (0965346160) vừa đặt đơn hàng OTO-FFFB. Tổng tiền: 158,900đ.', N'/admin/orders/detail/60', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-04T02:44:21.3916770' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2096, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-7B86. Tổng tiền: 208,800đ.', N'/admin/orders/detail/61', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-04T02:53:11.0931442' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2097, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-66B7. Tổng tiền: 158,900đ.', N'/admin/orders/detail/62', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-04T02:57:18.0816939' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2098, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-8981. Tổng tiền: 158,900đ.', N'/admin/orders/detail/63', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-04T02:59:57.4502170' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2099, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng WwangVinh04 (3814846150) vừa đặt đơn hàng OTO-BBEA. Tổng tiền: 194,437,900đ.', N'/admin/orders/detail/64', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-04T03:01:50.2556599' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2100, NULL, NULL, N'Cập nhật trạng thái đơn hàng 🔄', N'Đơn hàng OTO-BBEA vừa được chuyển sang trạng thái: Confirmed.', N'/admin/orders/detail/64', N'Manager', 0, CAST(N'2026-05-04T08:25:48.2282830' AS DateTime2), N'OrderStatus')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2101, NULL, NULL, N'Cập nhật trạng thái đơn hàng 🔄', N'Đơn hàng OTO-631A vừa được chuyển sang trạng thái: Confirmed.', N'/admin/orders/detail/56', N'Manager', 0, CAST(N'2026-05-04T08:26:21.3903523' AS DateTime2), N'OrderStatus')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2102, NULL, NULL, N'Tiền về tài khoản! 💰', N'Đơn hàng OTO-631A vừa được ghi nhận thanh toán 143đ qua Chuyển khoản. Trạng thái: Đã cọc.', N'/admin/orders/detail/56', N'Manager,Admin', 1, CAST(N'2026-05-04T08:26:51.9040109' AS DateTime2), N'Payment')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2103, NULL, NULL, N'Cập nhật trạng thái đơn hàng 🔄', N'Đơn hàng OTO-631A vừa được chuyển sang trạng thái: Completed.', N'/admin/orders/detail/56', N'Manager', 0, CAST(N'2026-05-04T08:27:02.9237780' AS DateTime2), N'OrderStatus')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2104, NULL, NULL, N'Cập nhật trạng thái đơn hàng 🔄', N'Đơn hàng OTO-BBEA vừa được chuyển sang trạng thái: Completed.', N'/admin/orders/detail/64', N'Manager', 0, CAST(N'2026-05-04T08:29:03.5382568' AS DateTime2), N'OrderStatus')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2105, NULL, NULL, N'Cập nhật trạng thái đơn hàng 🔄', N'Đơn hàng OTO-8981 vừa được chuyển sang trạng thái: Confirmed.', N'/admin/orders/detail/63', N'Manager', 0, CAST(N'2026-05-04T08:29:11.9729575' AS DateTime2), N'OrderStatus')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2106, NULL, NULL, N'Tiền về tài khoản! 💰', N'Đơn hàng OTO-8981 vừa được ghi nhận thanh toán 15,890đ qua Chuyển khoản. Trạng thái: Đã cọc.', N'/admin/orders/detail/63', N'Manager,Admin', 1, CAST(N'2026-05-04T08:29:27.7064009' AS DateTime2), N'Payment')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2107, NULL, NULL, N'Tiền về tài khoản! 💰', N'Đơn hàng OTO-8981 vừa được ghi nhận thanh toán 47,670đ qua Chuyển khoản. Trạng thái: Đã cọc.', N'/admin/orders/detail/63', N'Manager,Admin', 1, CAST(N'2026-05-04T08:29:39.8654270' AS DateTime2), N'Payment')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2108, NULL, NULL, N'Tiền về tài khoản! 💰', N'Đơn hàng OTO-8981 vừa được ghi nhận thanh toán 95đ qua Chuyển khoản. Trạng thái: Đã cọc.', N'/admin/orders/detail/63', N'Manager,Admin', 1, CAST(N'2026-05-04T08:29:56.8892147' AS DateTime2), N'Payment')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2109, NULL, NULL, N'Tiền về tài khoản! 💰', N'Đơn hàng OTO-8981 vừa được ghi nhận thanh toán 158,900đ qua Chuyển khoản. Trạng thái: Đã thanh toán đủ.', N'/admin/orders/detail/63', N'Manager,Admin', 1, CAST(N'2026-05-04T08:30:14.1324632' AS DateTime2), N'Payment')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (2110, NULL, NULL, N'Cập nhật trạng thái đơn hàng 🔄', N'Đơn hàng OTO-A200 vừa được chuyển sang trạng thái: Cancelled.', N'/admin/orders/detail/48', N'Manager', 0, CAST(N'2026-05-04T08:31:56.8671007' AS DateTime2), N'OrderStatus')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3107, NULL, 2, N'Lịch hẹn đã được tư vấn ✅', N'Lịch của khách WwangVinh04 (03814846150) - xe VinFast VF 9 Plus đã được tư vấn xong. Cần kiểm tra xe và xác nhận lịch lái thử.', N'/bookings', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-06T04:10:17.6623870' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3108, NULL, 2, N'Xe cần kiểm tra kỹ thuật 🔧', N'Xe VinFast VF 9 Plus cần kiểm tra trước lịch lái thử của khách WwangVinh04 vào 01/05/2026 lúc 23:39.', N'/bookings', N'Technician', 0, CAST(N'2026-05-06T04:10:22.5431774' AS DateTime2), N'TechCheck')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3109, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Nguyễn Hữu Thiện (0379748675) vừa đặt đơn hàng OTO-9F26. Tổng tiền: 208,800đ.', N'/admin/orders/detail/65', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-06T04:16:20.1343334' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3110, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Nguyễn Hữu Thiện (0379748675) vừa đặt đơn hàng OTO-5350. Tổng tiền: 208,800đ.', N'/admin/orders/detail/66', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-06T04:16:25.9131992' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3111, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Phùng Thế Tài (0965346160) vừa đặt đơn hàng OTO-53C2. Tổng tiền: 1,589,000,000đ.', N'/admin/orders/detail/67', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-06T04:17:17.9204020' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3112, NULL, NULL, N'Cập nhật trạng thái đơn hàng 🔄', N'Đơn hàng OTO-53C2 vừa được chuyển sang trạng thái: Confirmed.', N'/admin/orders/detail/67', N'Manager', 0, CAST(N'2026-05-06T04:18:44.5221327' AS DateTime2), N'OrderStatus')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3113, NULL, NULL, N'Tiền về tài khoản! 💰', N'Đơn hàng OTO-53C2 vừa được ghi nhận thanh toán 1,589,000,000đ qua Chuyển khoản. Trạng thái: Đã thanh toán đủ.', N'/admin/orders/detail/67', N'Manager,Admin', 1, CAST(N'2026-05-06T04:19:00.7487246' AS DateTime2), N'Payment')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3114, NULL, NULL, N'Cập nhật trạng thái đơn hàng 🔄', N'Đơn hàng OTO-53C2 vừa được chuyển sang trạng thái: Completed.', N'/admin/orders/detail/67', N'Manager', 0, CAST(N'2026-05-06T04:19:07.5932240' AS DateTime2), N'OrderStatus')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3115, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng vinh (0965346160) vừa đặt đơn hàng OTO-DBAF. Tổng tiền: 2,342,159,000đ.', N'/admin/orders/detail/68', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-06T04:42:54.0369618' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3116, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng vinh (0965346160) vừa đặt đơn hàng OTO-67E1. Tổng tiền: 2,342,159,000đ.', N'/admin/orders/detail/69', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-06T04:55:04.7182996' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3117, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Nguyễn Hữu Thiện (0379748675) vừa đặt đơn hàng OTO-6F41. Tổng tiền: 254,367,800đ.', N'/admin/orders/detail/70', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-06T12:19:36.5409220' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3118, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Nguyễn Hữu Thiện (0379748675) vừa đặt đơn hàng OTO-2102. Tổng tiền: 208,800đ.', N'/admin/orders/detail/71', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-06T12:19:54.2508173' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3119, NULL, NULL, N'Tiền về tài khoản! 💰', N'Đơn hàng OTO-2102 vừa được ghi nhận thanh toán 20,880đ qua PayOS (QR Code). Trạng thái: Đã cọc.', N'/admin/orders/detail/71', N'Manager,Admin', 1, CAST(N'2026-05-06T12:21:40.0407662' AS DateTime2), N'Payment')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3120, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Nguyễn Hữu Thiện (0379748675) vừa đặt đơn hàng OTO-ADA2. Tổng tiền: 158,900đ.', N'/admin/orders/detail/72', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-07T16:10:32.9418734' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3121, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Nguyễn Hữu Thiện (0379748675) vừa đặt đơn hàng OTO-3D83. Tổng tiền: 254,367,800đ.', N'/admin/orders/detail/73', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-07T16:11:02.0470743' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3122, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Nguyễn Hữu Thiện (0379748675) vừa đặt đơn hàng OTO-BCE8. Tổng tiền: 194,437,900đ.', N'/admin/orders/detail/74', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-07T16:13:20.3620662' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3123, NULL, NULL, N'Tiền về tài khoản! 💰', N'Đơn hàng OTO-BCE8 vừa được ghi nhận thanh toán 194,437,900đ qua Chuyển khoản. Trạng thái: Đã thanh toán đủ.', N'/admin/orders/detail/74', N'Manager,Admin', 1, CAST(N'2026-05-07T16:27:08.5792885' AS DateTime2), N'Payment')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3124, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Nguyễn Hữu Thiện (0379748675) vừa đặt đơn hàng OTO-B2D2. Tổng tiền: 2,088,000,000đ.', N'/admin/orders/detail/75', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-07T16:31:35.2614154' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3125, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Nguyễn Hữu Thiện (0379748675) vừa đặt đơn hàng OTO-7F2A. Tổng tiền: 1,783,279,000đ.', N'/admin/orders/detail/76', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-07T18:00:13.2247700' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3126, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Nguyễn Hữu Thiện (0379748675) vừa đặt đơn hàng OTO-E164. Tổng tiền: 2,342,159,000đ.', N'/admin/orders/detail/77', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-07T18:03:31.5889387' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3127, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Nguyễn Hữu Thiện (0379748675) vừa đặt đơn hàng OTO-779A. Tổng tiền: 2,088,000,000đ.', N'/admin/orders/detail/78', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-07T18:03:43.7467741' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3128, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng Nguyễn Hữu Thiện (0379748675) vừa đặt đơn hàng OTO-4755. Tổng tiền: 1,783,279,000đ.', N'/admin/orders/detail/79', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-07T21:01:25.5827780' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3129, NULL, NULL, N'Tưng bừng khai trương chi nhánh mới! 🎊', N'Công ty vừa mở thêm Showroom tại Ba Đình, Hà Nội. Chúc công ty ngày càng phát triển!', N'/admin/showrooms', NULL, 0, CAST(N'2026-05-07T21:08:01.5878630' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3130, NULL, NULL, N'Yêu cầu ký gửi xe mới 🚗', N'Khách Nguyễn Hữu Thiện (0379748675) muốn ký gửi TEST VF3 (2025). Vào kiểm tra ngay!', N'/consignments', N'Manager', 0, CAST(N'2026-05-07T21:09:08.1077662' AS DateTime2), N'System')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3131, NULL, 3, N'Có lịch hẹn lái thử mới! 📅', N'Khách èt (03814846150) vừa đặt lịch lái thử VINFAST VinFast VF9 lúc 21:50 ngày 07/05/2026.', N'/bookings', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-07T21:50:38.0810611' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3132, NULL, 3, N'Lịch hẹn đã được tư vấn ✅', N'Lịch của khách èt (03814846150) - xe VinFast VF9 đã được tư vấn xong. Cần kiểm tra xe và xác nhận lịch lái thử.', N'/bookings', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-07T21:50:53.8407000' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3133, NULL, 3, N'Xe cần kiểm tra kỹ thuật 🔧', N'Xe VinFast VF9 cần kiểm tra trước lịch lái thử của khách èt vào 07/05/2026 lúc 21:50.', N'/bookings', N'Technician', 0, CAST(N'2026-05-07T21:50:58.7818000' AS DateTime2), N'TechCheck')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3134, NULL, 3, N'Xe đã sẵn sàng ✅', N'Xe VinFast VF9 đã qua kiểm tra kỹ thuật. Vui lòng xác nhận lịch lái thử với khách èt.', N'/bookings', N'Sales,ShowroomSales,Manager', 0, CAST(N'2026-05-07T21:51:02.4330685' AS DateTime2), N'TechCheck')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3135, NULL, 3, N'Lịch lái thử đã được xác nhận 🎉', N'Lịch lái thử xe VinFast VF9 của khách èt (03814846150) đã xác nhận vào 07/05/2026 lúc 21:50.', N'/bookings', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-07T21:51:04.5026915' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3136, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng 1234456 (0379748675) vừa đặt đơn hàng OTO-D437. Tổng tiền: 1,589,000,000đ.', N'/admin/orders/detail/80', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-08T01:03:40.7059679' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3137, NULL, 1, N'Có lịch hẹn lái thử mới! 📅', N'Khách WwangVinh04 (0965346160) vừa đặt lịch lái thử VINFAST VinFast VF 9 Plus lúc 08:30 ngày 08/05/2026.', N'/bookings', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-08T08:30:17.6848894' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3138, NULL, 1, N'Có lịch hẹn lái thử mới! 📅', N'Khách WwangVinh04 (0965346160) vừa đặt lịch lái thử VINFAST VinFast VF 9 Plus lúc 19:51 ngày 08/05/2026.', N'/bookings', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-08T19:51:53.4622104' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (3139, NULL, NULL, N'Có đơn đặt xe mới! 🛒', N'Khách hàng sdcfvgbn (1234657823456) vừa đặt đơn hàng OTO-6317. Tổng tiền: 1,589,000,000đ.', N'/admin/orders/detail/81', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-08T19:53:34.7885740' AS DateTime2), N'Order')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (4137, NULL, 1, N'Có lịch hẹn lái thử mới! 📅', N'Khách WwangVinh04 (0965346160) vừa đặt lịch lái thử VINFAST VinFast VF 9 Plus lúc 23:00 ngày 11/05/2026.', N'/bookings', N'Manager,Sales,ShowroomSales', 1, CAST(N'2026-05-11T23:00:10.8556675' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (4138, NULL, 1, N'Lịch hẹn đã được tư vấn ✅', N'Lịch của khách WwangVinh04 (0965346160) - xe VinFast VF 9 Plus đã được tư vấn xong. Cần kiểm tra xe và xác nhận lịch lái thử.', N'/bookings', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-12T00:50:10.1457120' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (4139, NULL, 1, N'Xe cần kiểm tra kỹ thuật 🔧', N'Xe VinFast VF 9 Plus cần kiểm tra trước lịch lái thử của khách WwangVinh04 vào 11/05/2026 lúc 23:00.', N'/bookings', N'Technician', 0, CAST(N'2026-05-12T00:50:11.1978971' AS DateTime2), N'TechCheck')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (4140, NULL, 1, N'Có lịch hẹn lái thử mới! 📅', N'Khách WwangVinh04 (3814846150) vừa đặt lịch lái thử VINFAST VinFast VF 9 Plus lúc 08:58 ngày 28/05/2026.', N'/bookings', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-12T08:53:33.0739789' AS DateTime2), N'Booking')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (4141, NULL, 1, N'Xe đã sẵn sàng ✅', N'Xe VinFast VF 9 Plus đã qua kiểm tra kỹ thuật. Vui lòng xác nhận lịch lái thử với khách WwangVinh04.', N'/bookings', N'Sales,ShowroomSales,Manager', 0, CAST(N'2026-05-12T08:54:31.3131507' AS DateTime2), N'TechCheck')
GO
INSERT [dbo].[Notifications] ([NotificationId], [UserId], [ShowroomId], [Title], [Content], [ActionUrl], [RoleTarget], [IsRead], [CreatedAt], [NotificationType]) VALUES (4142, NULL, 1, N'Lịch lái thử đã được xác nhận 🎉', N'Lịch lái thử xe VinFast VF 9 Plus của khách WwangVinh04 (0965346160) đã xác nhận vào 11/05/2026 lúc 23:00.', N'/bookings', N'Manager,Sales,ShowroomSales', 0, CAST(N'2026-05-12T08:54:51.1775733' AS DateTime2), N'Booking')
GO
SET IDENTITY_INSERT [dbo].[Notifications] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderItems] ON 
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (2, 4, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (3, 5, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (4, 6, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (5, 7, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (6, 8, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (7, 9, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (8, 10, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (9, 11, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (10, 12, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (11, 13, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (12, 14, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (13, 15, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (14, 16, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (15, 17, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (16, 18, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (17, 19, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (18, 20, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (19, 21, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (20, 22, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (21, 23, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (22, 24, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (23, 25, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (24, 26, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (25, 27, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (26, 28, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (27, 29, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (28, 30, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (29, 31, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (30, 32, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (31, 33, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (32, 34, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (33, 35, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (34, 36, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (35, 37, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (36, 38, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (37, 39, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (38, 40, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (39, 41, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (40, 42, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (41, 43, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (42, 44, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (43, 45, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (44, 46, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (45, 47, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (46, 48, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (47, 49, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (48, 50, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (49, 51, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (50, 52, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (51, 53, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (52, 54, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (53, 55, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (54, 56, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (55, 57, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (56, 58, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (57, 59, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (58, 60, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (59, 61, 11, 1, CAST(2088000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (60, 62, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (61, 63, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (62, 64, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (63, 65, 11, 1, CAST(2088000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (64, 66, 11, 1, CAST(2088000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (65, 67, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (66, 68, 11, 1, CAST(2088000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (67, 69, 11, 1, CAST(2088000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (68, 70, 11, 1, CAST(2088000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (69, 71, 11, 1, CAST(2088000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (70, 72, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (71, 73, 11, 1, CAST(2088000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (72, 74, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (73, 75, 11, 1, CAST(2088000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (74, 76, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (75, 77, 11, 1, CAST(2088000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (76, 78, 11, 1, CAST(2088000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (77, 79, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (78, 80, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [CarId], [Quantity], [Price], [AccessoryId], [ItemType]) VALUES (79, 81, 11, 1, CAST(1589000000.00 AS Decimal(18, 2)), NULL, N'Car')
GO
SET IDENTITY_INSERT [dbo].[OrderItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (4, 11, CAST(N'2026-05-03T03:05:00.023' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-E777', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', NULL, N'37204783-7cbf-4064-80c5-6d0bcc8d3e9e', NULL, CAST(N'2026-05-03T03:05:00.023' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (5, 11, CAST(N'2026-05-03T13:51:44.350' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-3011', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', NULL, N'e4f33ffb-b73e-4ab0-8b2f-3073e23d91c0', NULL, CAST(N'2026-05-03T13:51:44.350' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (6, 11, CAST(N'2026-05-03T14:32:16.283' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-A7A3', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'cc4d6f90-5d33-4f66-a923-8ccf9446ef01', NULL, CAST(N'2026-05-03T14:32:16.283' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (7, 11, CAST(N'2026-05-03T14:32:20.043' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-3A09', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'826f6237-0306-4f72-b629-7dee85b69940', NULL, CAST(N'2026-05-03T14:32:20.043' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (8, 11, CAST(N'2026-05-03T14:32:20.770' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-7580', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'abe8329c-2bf6-4f03-bdc1-69019e4154e3', NULL, CAST(N'2026-05-03T14:32:20.770' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (9, 11, CAST(N'2026-05-03T14:32:20.910' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-0549', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'9aaa0a7a-7e62-4009-bc66-af496d02c1bd', NULL, CAST(N'2026-05-03T14:32:20.910' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (10, 11, CAST(N'2026-05-03T14:32:21.080' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-02DC', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'a355edbf-dec9-407b-9ab6-1ccad0da0819', NULL, CAST(N'2026-05-03T14:32:21.080' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (11, 11, CAST(N'2026-05-03T14:32:22.820' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-D557', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'29f6df33-9666-4e59-b050-ba29ef3c601d', NULL, CAST(N'2026-05-03T14:32:22.820' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (12, 11, CAST(N'2026-05-03T14:32:22.980' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-54A9', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'5968b37a-a3dd-4bab-885e-669ec5e0b28d', NULL, CAST(N'2026-05-03T14:32:22.980' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (13, 11, CAST(N'2026-05-03T14:32:25.190' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-D958', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'826abfd8-620f-426e-ae67-2b8e217c005c', NULL, CAST(N'2026-05-03T14:32:25.190' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (14, 11, CAST(N'2026-05-03T14:32:25.357' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-B8FD', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'460427d5-b882-4ae8-82a8-441710933a16', NULL, CAST(N'2026-05-03T14:32:25.357' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (15, 11, CAST(N'2026-05-03T14:32:25.507' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-0B11', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'c8158a19-3119-4fd5-9ec0-3f74b7db2414', NULL, CAST(N'2026-05-03T14:32:25.507' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (16, 11, CAST(N'2026-05-03T14:32:25.660' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-220D', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'9d6116a0-c281-4b9f-ba1c-84dfd8ad684b', NULL, CAST(N'2026-05-03T14:32:25.660' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (17, 11, CAST(N'2026-05-03T14:32:25.790' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-F158', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'c57a0131-f72e-4078-93ff-63b1e0a767d0', NULL, CAST(N'2026-05-03T14:32:25.790' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (18, 11, CAST(N'2026-05-03T14:32:31.703' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-9207', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'6484e00c-d4bd-44b7-a30a-85a013bd9273', NULL, CAST(N'2026-05-03T14:32:31.703' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (19, 11, CAST(N'2026-05-03T14:45:38.923' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-0D8A', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'609f548e-49da-4364-bde4-6c0b5eb832c2', NULL, CAST(N'2026-05-03T14:45:38.923' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (20, 11, CAST(N'2026-05-03T14:45:39.503' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-99AA', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'9f0aa13d-cb10-40d6-80e0-bfbdffb228c5', NULL, CAST(N'2026-05-03T14:45:39.503' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (21, 11, CAST(N'2026-05-03T14:45:39.660' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-C461', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'c876d811-8634-4255-81a0-0ef96686e3a1', NULL, CAST(N'2026-05-03T14:45:39.660' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (22, 11, CAST(N'2026-05-03T14:45:39.960' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-E3F2', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'e0cbe5d9-fb38-4f40-bb85-e2846f7370cf', NULL, CAST(N'2026-05-03T14:45:39.960' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (23, 11, CAST(N'2026-05-03T14:45:40.120' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-FB95', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'246210b3-caf3-4200-9e54-6870ec55ae84', NULL, CAST(N'2026-05-03T14:45:40.120' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (24, 11, CAST(N'2026-05-03T14:45:40.270' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-E277', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'cf17c3bf-8505-4b10-a955-3515ff6fab01', NULL, CAST(N'2026-05-03T14:45:40.270' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (25, 11, CAST(N'2026-05-03T14:45:40.427' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-BC44', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'0840f3f9-7d8c-4bb4-856b-824e2bf8126f', NULL, CAST(N'2026-05-03T14:45:40.427' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (26, 11, CAST(N'2026-05-03T14:45:40.577' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-5BB8', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'6b2b53fc-7eca-49a6-bb6f-b556fe02680f', NULL, CAST(N'2026-05-03T14:45:40.577' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (27, 11, CAST(N'2026-05-03T14:45:40.720' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-637E', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'70b2f0e8-ef16-42ea-950a-5aaa2b6e0616', NULL, CAST(N'2026-05-03T14:45:40.720' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (28, 11, CAST(N'2026-05-03T14:45:40.857' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-E19C', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'88cb11bf-847d-4337-a0f6-a733ac56dc06', NULL, CAST(N'2026-05-03T14:45:40.857' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (29, 11, CAST(N'2026-05-03T14:52:17.227' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-C5F3', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'9efd3af4-a5a3-4627-9964-e056a9ff3d9d', NULL, CAST(N'2026-05-03T14:52:17.227' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (30, 11, CAST(N'2026-05-03T14:53:13.340' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-FAD8', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'976f39a9-c5ed-4de5-8876-b0645321a386', NULL, CAST(N'2026-05-03T14:53:13.340' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (31, 11, CAST(N'2026-05-03T14:54:50.953' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-BE60', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'095b1793-76a5-4b50-83d8-b263f332b336', NULL, CAST(N'2026-05-03T14:54:50.953' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (32, 11, CAST(N'2026-05-03T14:58:20.540' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-73E0', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'ec665a38-d19a-4657-bf20-2f4c3d77aba0', NULL, CAST(N'2026-05-03T14:58:20.540' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (33, 11, CAST(N'2026-05-03T14:58:23.980' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-4B06', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'8931bf20-da19-400e-8182-aa64ef50a541', NULL, CAST(N'2026-05-03T14:58:23.980' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (34, 11, CAST(N'2026-05-03T14:58:49.017' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-1CE7', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'ae1b8352-42ba-47ad-a88e-3584652ad046', NULL, CAST(N'2026-05-03T14:58:49.017' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (35, 11, CAST(N'2026-05-03T15:06:57.083' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-357B', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'7bd27ff4-3239-4cbf-aa84-1169a4d6e75b', NULL, CAST(N'2026-05-03T15:06:57.080' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (36, 11, CAST(N'2026-05-03T15:07:00.983' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-4802', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'f15cbe0e-4dab-4052-bb55-ef1ac88c08ab', NULL, CAST(N'2026-05-03T15:07:00.983' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (37, 11, CAST(N'2026-05-03T15:07:12.903' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-6C25', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'84fe5efc-690e-4ca7-b2d3-2fe9a42896b9', NULL, CAST(N'2026-05-03T15:07:12.903' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (38, 11, CAST(N'2026-05-03T15:07:22.703' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-A8C3', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'f066aaa6-5a9c-4b69-a5a7-24cdfca76d0d', NULL, CAST(N'2026-05-03T15:07:22.703' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (39, 11, CAST(N'2026-05-03T15:10:04.267' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-B10A', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'12515a98-adb9-49cb-823d-949ffd60c708', NULL, CAST(N'2026-05-03T15:10:04.267' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (40, 11, CAST(N'2026-05-03T15:16:22.503' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-7C76', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'wangvinh', N'0379748675', N'vinhdghd@gmail.com', N'string', N'3bcbc4ae-79b4-4edd-950e-2e896d388aab', NULL, CAST(N'2026-05-03T15:16:22.503' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (41, 11, CAST(N'2026-05-03T15:22:51.170' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-0771', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'0ab10c15-ee4c-4379-92f8-6a0082b0366e', NULL, CAST(N'2026-05-03T15:22:51.170' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (42, 11, CAST(N'2026-05-03T15:26:29.137' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-798F', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'9912b449-02c6-4c99-ba16-9707f2fa790c', NULL, CAST(N'2026-05-03T15:26:29.137' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (43, 11, CAST(N'2026-05-03T15:27:04.833' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-E339', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'6c5bfc8e-214a-49b9-9d2b-ba68fe9af356', NULL, CAST(N'2026-05-03T15:27:04.833' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (44, 11, CAST(N'2026-05-03T15:27:59.780' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-2A9F', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'2c3c7eae-3363-47ce-9ac3-b4bd61f73bcb', NULL, CAST(N'2026-05-03T15:27:59.780' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (45, 11, CAST(N'2026-05-03T15:28:14.447' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-672B', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'4d2518fd-1f27-4e4e-9350-04c3d3503c28', NULL, CAST(N'2026-05-03T15:28:14.447' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (46, 11, CAST(N'2026-05-03T15:28:21.217' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-3072', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'3a9f2c30-72db-4927-a73e-65be58674e52', NULL, CAST(N'2026-05-03T15:28:21.217' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (47, 11, CAST(N'2026-05-03T15:33:14.053' AS DateTime), N'Pending', NULL, NULL, NULL, 11, N'OTO-4530', CAST(1589000000.00 AS Decimal(18, 2)), CAST(158900000.00 AS Decimal(18, 2)), CAST(1430100000.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'8fd14cd6-752b-48a7-9b99-9aaae269da3a', NULL, CAST(N'2026-05-03T15:33:14.053' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (48, 11, CAST(N'2026-05-03T19:47:11.447' AS DateTime), N'Cancelled', NULL, NULL, NULL, 3, N'OTO-A200', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Deposited', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'6ddc726a-b4a0-4203-b130-cb50bf93b3e3', N'', CAST(N'2026-05-04T08:31:56.857' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (49, 11, CAST(N'2026-05-03T20:24:16.143' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-267D', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'9fe16691-b3a5-41e4-9b57-0a4740105576', NULL, CAST(N'2026-05-03T20:24:16.143' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (50, 11, CAST(N'2026-05-03T21:22:58.903' AS DateTime), N'Cancelled', NULL, NULL, NULL, 3, N'OTO-4E31', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Unpaid', N'Lê Quang Vinh', N'0904059986', N'wwangvinh04@gmail.com', N'', N'2e00ce9f-180f-4d37-86a3-1ae942818f5d', NULL, CAST(N'2026-05-03T21:22:58.903' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (51, 11, CAST(N'2026-05-03T22:36:59.750' AS DateTime), N'Cancelled', NULL, NULL, NULL, NULL, N'OTO-8DA3', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'Lê Quang Vinh', N'0904059986', N'', N'', N'6d4c9dbd-6307-4237-b3f1-cd73bf386a80', NULL, CAST(N'2026-05-03T22:36:59.750' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (52, 11, CAST(N'2026-05-03T22:45:33.410' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-602A', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Unpaid', N'Lê Quang Vinh', N'0941756860', N'', N'', N'c0ca14ff-0e1a-48cc-b4bb-00b39d699161', NULL, CAST(N'2026-05-03T22:45:33.410' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (53, 11, CAST(N'2026-05-03T22:46:27.527' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-71C5', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Unpaid', N'Lê Quang Vinh', N'0941756860', N'', N'', N'36bbe9fc-b27b-4698-97c7-cc0bdef68314', NULL, CAST(N'2026-05-03T22:46:27.527' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (54, 11, CAST(N'2026-05-04T01:28:11.153' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-8856', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'e595c5a9-9753-4c04-b80f-6267788451e2', NULL, CAST(N'2026-05-04T01:28:11.153' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (55, 11, CAST(N'2026-05-04T01:28:27.417' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-B027', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'31d990c7-1f4b-4816-8b02-d6265b21f277', NULL, CAST(N'2026-05-04T01:28:27.417' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (56, 11, CAST(N'2026-05-04T01:40:58.607' AS DateTime), N'Completed', NULL, NULL, NULL, 3, N'OTO-631A', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Deposited', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'28ac83df-38e8-4091-a6e7-391b76c9f7fa', N'', CAST(N'2026-05-04T08:27:02.920' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (57, 11, CAST(N'2026-05-04T02:41:49.907' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-348B', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'0965346160', N'wwangvinh04@gmail.com', N'', N'ccd97a56-5ff0-4ed4-b1d8-dcdfcb1a0463', NULL, CAST(N'2026-05-04T02:41:49.907' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (58, 11, CAST(N'2026-05-04T02:41:57.440' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-6859', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'0965346160', N'wwangvinh04@gmail.com', N'', N'5dbc5a33-c76b-4e88-98c1-887d4956faa1', NULL, CAST(N'2026-05-04T02:41:57.440' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (59, 11, CAST(N'2026-05-04T02:43:57.927' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-A7D9', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'0965346160', N'wwangvinh04@gmail.com', N'', N'aa3d15d2-d71a-4d5e-8d55-0e5d017e224a', NULL, CAST(N'2026-05-04T02:43:57.927' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (60, 11, CAST(N'2026-05-04T02:44:21.357' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-FFFB', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'0965346160', N'wwangvinh04@gmail.com', N'', N'f1f6b42a-b040-4af8-8943-22e5eba66698', NULL, CAST(N'2026-05-04T02:44:21.357' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (61, 11, CAST(N'2026-05-04T02:53:10.890' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-7B86', CAST(2088000000.00 AS Decimal(18, 2)), CAST(2087791200.00 AS Decimal(18, 2)), CAST(208800.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'3bd62505-de4e-4ac6-8175-48c663a2bab3', NULL, CAST(N'2026-05-04T02:53:10.890' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (62, 11, CAST(N'2026-05-04T02:57:18.050' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-66B7', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'365f7210-4739-4f23-a2ce-cd034efdbeb0', NULL, CAST(N'2026-05-04T02:57:18.050' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (63, 11, CAST(N'2026-05-04T02:59:57.423' AS DateTime), N'Confirmed', NULL, NULL, NULL, 3, N'OTO-8981', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Paid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'4f42c6fe-e44f-462a-9731-b724df6139c4', N'', CAST(N'2026-05-04T08:30:14.130' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (64, 11, CAST(N'2026-05-04T03:01:49.700' AS DateTime), N'Completed', NULL, NULL, NULL, 3, N'OTO-BBEA', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(194437900.00 AS Decimal(18, 2)), N'Unpaid', N'WwangVinh04', N'3814846150', N'wwangvinh04@gmail.com', N'', N'006680c0-9814-459c-9062-069235b9c448', N'', CAST(N'2026-05-04T08:29:03.530' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (65, 11, CAST(N'2026-05-06T04:16:20.007' AS DateTime), N'Cancelled', NULL, NULL, NULL, 3, N'OTO-9F26', CAST(2088000000.00 AS Decimal(18, 2)), CAST(2087791200.00 AS Decimal(18, 2)), CAST(208800.00 AS Decimal(18, 2)), N'Unpaid', N'Nguyễn Hữu Thiện', N'0379748675', N'huuthien204@gmail.com', N'', N'c0afaad9-1dbf-4be6-b021-c5418b5786a4', NULL, CAST(N'2026-05-06T04:16:20.007' AS DateTime), NULL, 1)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (66, 11, CAST(N'2026-05-06T04:16:25.893' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-5350', CAST(2088000000.00 AS Decimal(18, 2)), CAST(2087791200.00 AS Decimal(18, 2)), CAST(208800.00 AS Decimal(18, 2)), N'Unpaid', N'Nguyễn Hữu Thiện', N'0379748675', N'huuthien204@gmail.com', N'', N'adee09b5-ac21-43b4-8c80-6b6710e4b349', NULL, CAST(N'2026-05-06T04:16:25.893' AS DateTime), NULL, 1)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (67, 11, CAST(N'2026-05-06T04:17:17.893' AS DateTime), N'Completed', NULL, NULL, NULL, NULL, N'OTO-53C2', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Paid', N'Phùng Thế Tài', N'0965346160', N'phungthetai@gmail.com', N'', N'50c857f4-c8c7-4fef-98e7-bbfa071adfd5', N'', CAST(N'2026-05-06T04:19:07.590' AS DateTime), NULL, 6)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (68, 11, CAST(N'2026-05-06T04:42:54.010' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-DBAF', CAST(2088000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(2342159000.00 AS Decimal(18, 2)), N'Unpaid', N'vinh', N'0965346160', N'wwangvinh04@gmail.com', N'', N'703c49a0-fccb-4ca8-a8a1-f923b36e3b16', NULL, CAST(N'2026-05-06T04:42:54.010' AS DateTime), NULL, 2)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (69, 11, CAST(N'2026-05-06T04:55:04.703' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-67E1', CAST(2088000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(2342159000.00 AS Decimal(18, 2)), N'Unpaid', N'vinh', N'0965346160', N'wwangvinh04@gmail.com', N'', N'132a550f-ccf0-4e28-9ec2-7ab239c92eae', NULL, CAST(N'2026-05-06T04:55:04.703' AS DateTime), NULL, 2)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (70, 11, CAST(N'2026-05-06T12:19:36.263' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-6F41', CAST(2088000000.00 AS Decimal(18, 2)), CAST(2087791200.00 AS Decimal(18, 2)), CAST(254367800.00 AS Decimal(18, 2)), N'Unpaid', N'Nguyễn Hữu Thiện', N'0379748675', N'huuthien204@gmail.com', N'', N'e7efd908-373a-4888-917b-18a1688a46a2', NULL, CAST(N'2026-05-06T12:19:36.263' AS DateTime), NULL, 2)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (71, 11, CAST(N'2026-05-06T12:19:54.223' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-2102', CAST(2088000000.00 AS Decimal(18, 2)), CAST(2087791200.00 AS Decimal(18, 2)), CAST(208800.00 AS Decimal(18, 2)), N'Deposited', N'Nguyễn Hữu Thiện', N'0379748675', N'huuthien204@gmail.com', N'', N'7105abba-ccc0-4d5f-a2e9-9a3cdd7c6789', NULL, CAST(N'2026-05-06T12:21:40.030' AS DateTime), NULL, 2)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (72, 11, CAST(N'2026-05-07T16:10:32.713' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-ADA2', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(158900.00 AS Decimal(18, 2)), N'Unpaid', N'Nguyễn Hữu Thiện', N'0379748675', N'huuthien204@gmail.com', N'', N'5cfe7ee6-c80f-4897-b1f7-fbf9bf02c66f', NULL, CAST(N'2026-05-07T16:10:32.713' AS DateTime), NULL, 6)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (73, 11, CAST(N'2026-05-07T16:11:02.000' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-3D83', CAST(2088000000.00 AS Decimal(18, 2)), CAST(2087791200.00 AS Decimal(18, 2)), CAST(254367800.00 AS Decimal(18, 2)), N'Unpaid', N'Nguyễn Hữu Thiện', N'0379748675', N'huuthien204@gmail.com', N'', N'6e218640-0829-410d-b295-654758267347', NULL, CAST(N'2026-05-07T16:11:02.000' AS DateTime), NULL, 1)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (74, 11, CAST(N'2026-05-07T16:13:20.333' AS DateTime), N'Pending', NULL, NULL, NULL, 3, N'OTO-BCE8', CAST(1589000000.00 AS Decimal(18, 2)), CAST(1588841100.00 AS Decimal(18, 2)), CAST(194437900.00 AS Decimal(18, 2)), N'Paid', N'Nguyễn Hữu Thiện', N'0379748675', N'huuthien204@gmail.com', N'', N'1819a553-f70b-4924-aba5-3f10006f01b3', NULL, CAST(N'2026-05-07T16:27:08.570' AS DateTime), NULL, 1)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (75, 11, CAST(N'2026-05-07T16:31:35.057' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-B2D2', CAST(2088000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(2088000000.00 AS Decimal(18, 2)), N'Unpaid', N'Nguyễn Hữu Thiện', N'0379748675', N'huuthien204@gmail.com', N'', N'508e2d60-4a3c-4f94-b1f4-2c0244c944a5', NULL, CAST(N'2026-05-07T16:31:35.053' AS DateTime), NULL, 6)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (76, 11, CAST(N'2026-05-07T18:00:13.197' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-7F2A', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1783279000.00 AS Decimal(18, 2)), N'Unpaid', N'Nguyễn Hữu Thiện', N'0379748675', N'huuthien204@gmail.com', N'', N'e9c3b0eb-8584-4feb-884a-dcee722d65d5', NULL, CAST(N'2026-05-07T18:00:13.197' AS DateTime), NULL, 2)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (77, 11, CAST(N'2026-05-07T18:03:31.563' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-E164', CAST(2088000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(2342159000.00 AS Decimal(18, 2)), N'Unpaid', N'Nguyễn Hữu Thiện', N'0379748675', N'huuthien204@gmail.com', N'', N'de0253d1-979e-4cfd-98da-003d396f3f0f', NULL, CAST(N'2026-05-07T18:03:31.563' AS DateTime), NULL, 2)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (78, 11, CAST(N'2026-05-07T18:03:43.733' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-779A', CAST(2088000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(2088000000.00 AS Decimal(18, 2)), N'Unpaid', N'Nguyễn Hữu Thiện', N'0379748675', N'huuthien204@gmail.com', N'', N'd99efcce-20b9-4793-9352-a64be168b01f', NULL, CAST(N'2026-05-07T18:03:43.733' AS DateTime), NULL, 2)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (79, 11, CAST(N'2026-05-07T21:01:25.563' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-4755', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1783279000.00 AS Decimal(18, 2)), N'Unpaid', N'Nguyễn Hữu Thiện', N'0379748675', N'huuthien204@gmail.com', N'', N'8c8ca47a-ac61-4b61-b742-e684be50ae0b', NULL, CAST(N'2026-05-07T21:01:25.563' AS DateTime), NULL, 6)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (80, 11, CAST(N'2026-05-08T01:03:40.663' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-D437', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'1234456', N'0379748675', N'', N'', N'13913226-9433-4d20-aa57-991778541736', NULL, CAST(N'2026-05-08T01:03:40.663' AS DateTime), NULL, 1)
GO
INSERT [dbo].[Orders] ([OrderId], [CarId], [OrderDate], [Status], [TotalAmount], [PaymentMethod], [ShippingAddress], [PromotionId], [OrderCode], [Subtotal], [DiscountAmount], [FinalAmount], [PaymentStatus], [FullName], [Phone], [Email], [CustomerNote], [SecretToken], [AdminNote], [LastUpdated], [StaffId], [ShowroomId]) VALUES (81, 11, CAST(N'2026-05-08T19:53:34.687' AS DateTime), N'Pending', NULL, NULL, NULL, NULL, N'OTO-6317', CAST(1589000000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(1589000000.00 AS Decimal(18, 2)), N'Unpaid', N'sdcfvgbn', N'1234657823456', N'wwangvinh04@gmail.com', N'', N'58d8a0ee-7114-44d1-a14e-09b9a1d74ac2', NULL, CAST(N'2026-05-08T19:53:34.687' AS DateTime), NULL, 7)
GO
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[PaymentTransactions] ON 
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (1, 56, CAST(15890.00 AS Decimal(18, 2)), N'PayOS (QR Code)', CAST(N'2026-05-04T02:28:40.207' AS DateTime), N'Success')
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (2, 48, CAST(15890.00 AS Decimal(18, 2)), N'PayOS (QR Code)', CAST(N'2026-05-04T02:29:38.573' AS DateTime), N'Success')
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (3, 56, CAST(143.01 AS Decimal(18, 2)), N'Chuyển khoản', CAST(N'2026-05-04T08:26:51.873' AS DateTime), N'Success')
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (4, 63, CAST(15890.00 AS Decimal(18, 2)), N'Chuyển khoản', CAST(N'2026-05-04T08:29:27.693' AS DateTime), N'Success')
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (5, 63, CAST(47670.00 AS Decimal(18, 2)), N'Chuyển khoản', CAST(N'2026-05-04T08:29:39.850' AS DateTime), N'Success')
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (6, 63, CAST(95.34 AS Decimal(18, 2)), N'Chuyển khoản', CAST(N'2026-05-04T08:29:56.880' AS DateTime), N'Success')
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (7, 63, CAST(158900.00 AS Decimal(18, 2)), N'Chuyển khoản', CAST(N'2026-05-04T08:30:14.123' AS DateTime), N'Success')
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (1006, 67, CAST(1589000000.00 AS Decimal(18, 2)), N'Chuyển khoản', CAST(N'2026-05-06T04:19:00.717' AS DateTime), N'Success')
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (1007, 71, CAST(20880.00 AS Decimal(18, 2)), N'PayOS (QR Code)', CAST(N'2026-05-06T12:21:40.010' AS DateTime), N'Success')
GO
INSERT [dbo].[PaymentTransactions] ([TransactionId], [OrderId], [Amount], [PaymentMethod], [TransactionDate], [Status]) VALUES (1008, 74, CAST(194437900.00 AS Decimal(18, 2)), N'Chuyển khoản', CAST(N'2026-05-07T16:27:08.537' AS DateTime), N'Success')
GO
SET IDENTITY_INSERT [dbo].[PaymentTransactions] OFF
GO
SET IDENTITY_INSERT [dbo].[Promotions] ON 
GO
INSERT [dbo].[Promotions] ([PromotionId], [PromotionName], [DiscountAmount], [Code], [DiscountPercentage], [StartDate], [EndDate], [Description], [Status], [CarId], [MaxUsage], [CurrentUsage]) VALUES (3, NULL, NULL, N'GIAM_GIA_KHUNG', CAST(99.99 AS Decimal(5, 2)), CAST(N'2026-04-25T14:50:00.000' AS DateTime), CAST(N'2026-05-25T14:50:00.000' AS DateTime), N'Giảm 100%', N'Active', NULL, 1000, 48)
GO
INSERT [dbo].[Promotions] ([PromotionId], [PromotionName], [DiscountAmount], [Code], [DiscountPercentage], [StartDate], [EndDate], [Description], [Status], [CarId], [MaxUsage], [CurrentUsage]) VALUES (11, NULL, NULL, N'SDFGHGF', CAST(10.00 AS Decimal(5, 2)), CAST(N'2026-04-29T06:28:00.000' AS DateTime), CAST(N'2026-05-06T06:28:00.000' AS DateTime), N'sv', N'Active', NULL, 100, 1)
GO
INSERT [dbo].[Promotions] ([PromotionId], [PromotionName], [DiscountAmount], [Code], [DiscountPercentage], [StartDate], [EndDate], [Description], [Status], [CarId], [MaxUsage], [CurrentUsage]) VALUES (12, NULL, NULL, N'SDAFDGSDFMG', CAST(15.00 AS Decimal(5, 2)), CAST(N'2026-04-29T06:54:00.000' AS DateTime), CAST(N'2026-05-06T06:54:00.000' AS DateTime), N'dwdwqdqwd', N'Active', NULL, 100, 0)
GO
SET IDENTITY_INSERT [dbo].[Promotions] OFF
GO
SET IDENTITY_INSERT [dbo].[Reviews] ON 
GO
INSERT [dbo].[Reviews] ([ReviewId], [CarId], [Rating], [Comment], [CreatedAt], [FullName], [Phone], [OrderCode], [IsApproved], [UserId]) VALUES (1, 11, 5, N'tuyệt
', CAST(N'2026-05-03T21:56:39.910' AS DateTime), N'Lê Vinh', N'0965346160', NULL, 1, NULL)
GO
SET IDENTITY_INSERT [dbo].[Reviews] OFF
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
INSERT [dbo].[Showrooms] ([ShowroomId], [Name], [Province], [District], [StreetAddress], [Hotline]) VALUES (7, N'Theiẹn', N'Hà Nội', N'Ba Đình', N'123 Hàng Bông', N'XXXXXXXXXX')
GO
SET IDENTITY_INSERT [dbo].[Showrooms] OFF
GO
INSERT [dbo].[SystemSettings] ([SettingKey], [SettingValue], [Description]) VALUES (N'DepositPercentage', N'10', N'Phần trăm đặt cọc mặc định khi mua xe')
GO
SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (1, N'admin', N'$2a$11$VJMt1twINyJGr7sGlOgBHuvluQ79RWP6XA5Ua3vB1qIUBXoJfMYT6', N'admin@gmail.com', N'Adminn', N'0939775683', N'Admin', NULL, CAST(N'2026-04-26T23:19:17.283' AS DateTime), N'Active', 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (2, N'vinhquanli', N'$2a$11$F5DBAFASl8G0zPe.rhXDOenyaMBE6Wqh.yvEhd5R5.S7X7901eIwi', N'wwangvinh04@gmail.com', N'Lê Quang Vinh', N'09xxxxxxxx', N'Manager', NULL, CAST(N'2026-04-27T10:59:13.907' AS DateTime), N'Active', 0, NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (3, N'vinh', N'$2a$11$ERsYLCreN8LJKVuVxYZS..reu5u4/cxU/6Pv57HhV7PS2tBskw3O6', N'wwangvinh004@gmail.com', N'Le Quang Vinh', N'090xxxxxxx', N'ShowroomSales', NULL, CAST(N'2026-04-27T11:00:03.947' AS DateTime), N'Active', 0, NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (8, N'vinhkithuat', N'$2a$11$XLCKGEbqoiZVHMwSgatc5.tiDJacJTPts8LoT9m99JLJywqcvIXIy', N'wwangvonh0004@gmail.com', N'Lee Quang Vinh', N'0904059986', N'Technician', NULL, CAST(N'2026-04-29T07:44:50.873' AS DateTime), N'Active', 0, NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (9, N'vinhkithuat2', N'$2a$11$hVp1lW8ySFuZNm1UcLRlGeJ5nEYlktCzpRb7qpp2US0YBvlyjtxKa', N'wwangvinh05@gmail.com', N'Le Van Vinh', N'0904059976', N'Technician', NULL, CAST(N'2026-04-29T11:02:01.050' AS DateTime), N'Active', 0, NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (10, N'vinhcontent', N'$2a$11$X6c9C5vitHnI52OmBQJP.eP3086NeHrKuM3QxV3CnP3Lj.z7fP9bm', N'wwangvinh06@gmail.com', N'Le Vann Vinh', N'0904059996', N'Sales', NULL, CAST(N'2026-04-29T12:31:42.823' AS DateTime), N'Active', 0, NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (11, N'VinhContent2', N'$2a$11$tgByEPYQzXd2Mv60h.qa3.12AMD58BOrVPROKvlUlUPkIU0SnSHki', N'wwangvinh08@gmail.com', N'Le Van Vinhh', N'0904059996', N'Content', NULL, CAST(N'2026-04-29T12:58:50.570' AS DateTime), N'Active', 0, NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (12, N'vinhmarketing', N'$2a$11$WBGtg40oucq87nIf/K67tuA4tmGy5/99J5qnbclKkz5efPwQnCnv6', N'wwangvinh07@gmail.com', N'Le Vann VInhh', N'0904079996', N'Marketing', NULL, CAST(N'2026-04-29T12:59:22.760' AS DateTime), N'Active', 0, NULL, NULL, NULL, 1)
GO
INSERT [dbo].[Users] ([UserId], [Username], [PasswordHash], [Email], [FullName], [Phone], [Role], [Address], [CreatedAt], [Status], [IsDeleted], [DeletedAt], [DeletedBy], [AvatarUrl], [ShowroomId]) VALUES (13, N'vinh2', N'$2a$11$445Wk1zTFYT13lxB.ltAy.HfA/cNgNA95fb3H64aV7f.lSNrDqnLe', N'wwangvinh09@gmail.com', N'Lê Quang Vinhh', N'0905940661', N'ShowroomSales', NULL, CAST(N'2026-05-02T20:47:40.857' AS DateTime), N'Active', 0, NULL, NULL, NULL, 3)
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
/****** Object:  Index [IX_AIRecommendations_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_AIRecommendations_CarId] ON [dbo].[AIRecommendations]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AIRecommendations_UserId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_AIRecommendations_UserId] ON [dbo].[AIRecommendations]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Articles_AuthorId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Articles_AuthorId] ON [dbo].[Articles]
(
	[AuthorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_CarId] ON [dbo].[Bookings]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_ShowroomId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_ShowroomId] ON [dbo].[Bookings]
(
	[ShowroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_UserId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_UserId] ON [dbo].[Bookings]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarAccessories_AccessoryId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_CarAccessories_AccessoryId] ON [dbo].[CarAccessories]
(
	[AccessoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarAccessories_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_CarAccessories_CarId] ON [dbo].[CarAccessories]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CarColors_CarId_ColorName]    Script Date: 6/8/2026 2:38:19 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_CarColors_CarId_ColorName] ON [dbo].[CarColors]
(
	[CarId] ASC,
	[ColorName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarFeatures_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_CarFeatures_CarId] ON [dbo].[CarFeatures]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarFeatures_FeatureId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_CarFeatures_FeatureId] ON [dbo].[CarFeatures]
(
	[FeatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarImages_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_CarImages_CarId] ON [dbo].[CarImages]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarInventories_CarColorId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_CarInventories_CarColorId] ON [dbo].[CarInventories]
(
	[CarColorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarInventories_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_CarInventories_CarId] ON [dbo].[CarInventories]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarInventories_ShowroomId_CarId_CarColorId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_CarInventories_ShowroomId_CarId_CarColorId] ON [dbo].[CarInventories]
(
	[ShowroomId] ASC,
	[CarId] ASC,
	[CarColorId] ASC
)
WHERE ([CarColorId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarPricingVersions_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_CarPricingVersions_CarId] ON [dbo].[CarPricingVersions]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Cars_CreatedByUserId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Cars_CreatedByUserId] ON [dbo].[Cars]
(
	[CreatedByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Cars_Status]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Cars_Status] ON [dbo].[Cars]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Cars_Status_IsDeleted]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Cars_Status_IsDeleted] ON [dbo].[Cars]
(
	[Status] ASC,
	[IsDeleted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarSpecifications_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_CarSpecifications_CarId] ON [dbo].[CarSpecifications]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarWishlist_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_CarWishlist_CarId] ON [dbo].[CarWishlist]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CarWishlist_UserId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_CarWishlist_UserId] ON [dbo].[CarWishlist]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChatMessages_SessionId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_ChatMessages_SessionId] ON [dbo].[ChatMessages]
(
	[SessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChatSessions_AssignedTo]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_ChatSessions_AssignedTo] ON [dbo].[ChatSessions]
(
	[AssignedTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChatSessions_UserId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_ChatSessions_UserId] ON [dbo].[ChatSessions]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Consignments_LinkedCarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Consignments_LinkedCarId] ON [dbo].[Consignments]
(
	[LinkedCarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConsultationProfiles_SessionId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_ConsultationProfiles_SessionId] ON [dbo].[ConsultationProfiles]
(
	[SessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConsultationProfiles_UserId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_ConsultationProfiles_UserId] ON [dbo].[ConsultationProfiles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConsultRequests_CarColorId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_ConsultRequests_CarColorId] ON [dbo].[ConsultRequests]
(
	[CarColorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConsultRequests_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_ConsultRequests_CarId] ON [dbo].[ConsultRequests]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConsultRequests_CarPricingVersionId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_ConsultRequests_CarPricingVersionId] ON [dbo].[ConsultRequests]
(
	[CarPricingVersionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConsultRequests_ShowroomId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_ConsultRequests_ShowroomId] ON [dbo].[ConsultRequests]
(
	[ShowroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ConsultRequests_UserId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_ConsultRequests_UserId] ON [dbo].[ConsultRequests]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Notifications_ShowroomId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Notifications_ShowroomId] ON [dbo].[Notifications]
(
	[ShowroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Notifications_UserId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Notifications_UserId] ON [dbo].[Notifications]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_CarId] ON [dbo].[OrderItems]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_OrderId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderId] ON [dbo].[OrderItems]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Orders_CarId] ON [dbo].[Orders]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_PromotionId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Orders_PromotionId] ON [dbo].[Orders]
(
	[PromotionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_ShowroomId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Orders_ShowroomId] ON [dbo].[Orders]
(
	[ShowroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PaymentTransactions_OrderId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_PaymentTransactions_OrderId] ON [dbo].[PaymentTransactions]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Promotio__A25C5AA7A444C1CD]    Script Date: 6/8/2026 2:38:19 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Promotio__A25C5AA7A444C1CD] ON [dbo].[Promotions]
(
	[Code] ASC
)
WHERE ([Code] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Reviews_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Reviews_CarId] ON [dbo].[Reviews]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SystemAuditLogs_UserId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_SystemAuditLogs_UserId] ON [dbo].[SystemAuditLogs]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserActivity_CarId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_UserActivity_CarId] ON [dbo].[UserActivity]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserActivity_UserId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_UserActivity_UserId] ON [dbo].[UserActivity]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserLogins_UserId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_UserLogins_UserId] ON [dbo].[UserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Users_ShowroomId]    Script Date: 6/8/2026 2:38:19 ******/
CREATE NONCLUSTERED INDEX [IX_Users_ShowroomId] ON [dbo].[Users]
(
	[ShowroomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__536C85E4014A9D7C]    Script Date: 6/8/2026 2:38:19 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ__Users__536C85E4014A9D7C] ON [dbo].[Users]
(
	[Username] ASC
)
WHERE ([Username] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D105348B415492]    Script Date: 6/8/2026 2:38:19 ******/
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
ALTER TABLE [dbo].[CarColors] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
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
ALTER TABLE [dbo].[Consignments] ADD  DEFAULT ((0.0)) FOR [ExpectedPrice]
GO
ALTER TABLE [dbo].[Consignments] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Consignments] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Consignments] ADD  DEFAULT (N'') FOR [GuestName]
GO
ALTER TABLE [dbo].[Consignments] ADD  DEFAULT (N'') FOR [GuestPhone]
GO
ALTER TABLE [dbo].[ConsultRequests] ADD  DEFAULT (N'Pending') FOR [Status]
GO
ALTER TABLE [dbo].[ConsultRequests] ADD  DEFAULT (getdate()) FOR [CreatedAt]
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
ALTER TABLE [dbo].[CarAccessories]  WITH CHECK ADD  CONSTRAINT [FK_CarAccessories_Accessories_AccessoryId] FOREIGN KEY([AccessoryId])
REFERENCES [dbo].[Accessories] ([AccessoryId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CarAccessories] CHECK CONSTRAINT [FK_CarAccessories_Accessories_AccessoryId]
GO
ALTER TABLE [dbo].[CarAccessories]  WITH CHECK ADD  CONSTRAINT [FK_CarAccessories_Cars_CarId] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CarAccessories] CHECK CONSTRAINT [FK_CarAccessories_Cars_CarId]
GO
ALTER TABLE [dbo].[CarColors]  WITH CHECK ADD  CONSTRAINT [FK_CarColors_Cars_CarId] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[CarColors] CHECK CONSTRAINT [FK_CarColors_Cars_CarId]
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
ALTER TABLE [dbo].[CarInventories]  WITH CHECK ADD  CONSTRAINT [FK_CarInventories_CarColors_CarColorId] FOREIGN KEY([CarColorId])
REFERENCES [dbo].[CarColors] ([CarColorId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[CarInventories] CHECK CONSTRAINT [FK_CarInventories_CarColors_CarColorId]
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
ALTER TABLE [dbo].[Cars]  WITH CHECK ADD  CONSTRAINT [FK_Cars_Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Cars] CHECK CONSTRAINT [FK_Cars_Users_CreatedByUserId]
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
ALTER TABLE [dbo].[ConsultRequests]  WITH CHECK ADD  CONSTRAINT [FK_ConsultRequests_CarColors_CarColorId] FOREIGN KEY([CarColorId])
REFERENCES [dbo].[CarColors] ([CarColorId])
GO
ALTER TABLE [dbo].[ConsultRequests] CHECK CONSTRAINT [FK_ConsultRequests_CarColors_CarColorId]
GO
ALTER TABLE [dbo].[ConsultRequests]  WITH CHECK ADD  CONSTRAINT [FK_ConsultRequests_CarPricingVersions_PricingVersionId] FOREIGN KEY([CarPricingVersionId])
REFERENCES [dbo].[CarPricingVersions] ([PricingVersionId])
GO
ALTER TABLE [dbo].[ConsultRequests] CHECK CONSTRAINT [FK_ConsultRequests_CarPricingVersions_PricingVersionId]
GO
ALTER TABLE [dbo].[ConsultRequests]  WITH CHECK ADD  CONSTRAINT [FK_ConsultRequests_Cars_CarId] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([CarId])
GO
ALTER TABLE [dbo].[ConsultRequests] CHECK CONSTRAINT [FK_ConsultRequests_Cars_CarId]
GO
ALTER TABLE [dbo].[ConsultRequests]  WITH CHECK ADD  CONSTRAINT [FK_ConsultRequests_Showrooms_ShowroomId] FOREIGN KEY([ShowroomId])
REFERENCES [dbo].[Showrooms] ([ShowroomId])
GO
ALTER TABLE [dbo].[ConsultRequests] CHECK CONSTRAINT [FK_ConsultRequests_Showrooms_ShowroomId]
GO
ALTER TABLE [dbo].[ConsultRequests]  WITH CHECK ADD  CONSTRAINT [FK_ConsultRequests_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ConsultRequests] CHECK CONSTRAINT [FK_ConsultRequests_Users_UserId]
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
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Showrooms_ShowroomId] FOREIGN KEY([ShowroomId])
REFERENCES [dbo].[Showrooms] ([ShowroomId])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Showrooms_ShowroomId]
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
