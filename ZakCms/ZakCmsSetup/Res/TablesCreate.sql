CREATE TABLE [dbo].[Languages](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [char](5) COLLATE Latin1_General_CI_AS NOT NULL UNIQUE,
	[Description] [nvarchar](max) COLLATE Latin1_General_CI_AS NOT NULL
)

GO

CREATE TABLE [dbo].[Tags](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL UNIQUE,
	[Description] [nvarchar](max) COLLATE Latin1_General_CI_AS NOT NULL
)

CREATE TABLE [dbo].[FEUsers](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL UNIQUE,
	[UserPassword] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL
)
GO

CREATE TABLE [dbo].[BEUsers](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL UNIQUE,
	[UserPassword] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL
)
GO

CREATE TABLE [dbo].[Companies](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [char](5) COLLATE Latin1_General_CI_AS NOT NULL UNIQUE,
	[Description] [nvarchar](max) COLLATE Latin1_General_CI_AS NOT NULL
)
GO

CREATE TABLE [dbo].[Feeds](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](max) COLLATE Latin1_General_CI_AS NOT NULL,
	[LanguageId] [bigint] NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[SeoTitle] [nvarchar](255) COLLATE Latin1_General_CI_AS NULL,
	[CreateTime] DATETIME DEFAULT(GETDATE()) NOT NULL,
	[UpdateTime] DATETIME DEFAULT(GETDATE()) NOT NULL
)
GO

CREATE TABLE [dbo].[Articles](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](max) COLLATE Latin1_General_CI_AS NOT NULL,
	[Content] [nvarchar](max) COLLATE Latin1_General_CI_AS NOT NULL,
	[ParentId] [bigint] NOT NULL,
	[Ordering] [int] NOT NULL,
	[LeftNs] [bigint] NOT NULL,
	[RightNs] [bigint] NOT NULL,
	[LanguageId] [bigint] NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[IsAuthenticated] [bit] DEFAULT(0) NOT NULL,
	[SeoTitle] [nvarchar](255) COLLATE Latin1_General_CI_AS NULL,
	[CreateTime] DATETIME DEFAULT(GETDATE()) NOT NULL,
	[UpdateTime] DATETIME DEFAULT(GETDATE()) NOT NULL
)
GO

CREATE TABLE [dbo].[ArticlesToTags](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[LeftId] [bigint] NOT NULL,
	[RightId] [bigint] NOT NULL
)
GO

CREATE TABLE [dbo].[FeedsToTags](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[LeftId] [bigint] NOT NULL,
	[RightId] [bigint] NOT NULL
)
GO


CREATE TABLE [dbo].[FeedsContent](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FeedId] [bigint] NOT NULL,
	[SourceId] [bigint] NOT NULL,
	[SourceType] [bigint] NOT NULL,
	[Title] [nvarchar](max) COLLATE Latin1_General_CI_AS NOT NULL,
	[Content] [nvarchar](max) COLLATE Latin1_General_CI_AS NOT NULL,
	[SeoTitle] [nvarchar](255) COLLATE Latin1_General_CI_AS NULL,
	[CreateTime] DATETIME DEFAULT(GETDATE()) NOT NULL,
	[UpdateTime] DATETIME DEFAULT(GETDATE()) NOT NULL
)
GO