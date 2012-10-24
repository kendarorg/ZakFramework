
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Articles]') AND type in (N'U'))
TRUNCATE TABLE [dbo].[Articles]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Companies]') AND type in (N'U'))
TRUNCATE TABLE [dbo].[Companies]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Languages]') AND type in (N'U'))
TRUNCATE TABLE [dbo].[Languages]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tags]') AND type in (N'U'))
TRUNCATE TABLE [dbo].[Tags]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArticlesToTags]') AND type in (N'U'))
TRUNCATE TABLE [dbo].[ArticlesToTags]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FEUsers]') AND type in (N'U'))
TRUNCATE TABLE [dbo].[FEUsers]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BEUsers]') AND type in (N'U'))
TRUNCATE TABLE [dbo].[BEUsers]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Feeds]') AND type in (N'U'))
TRUNCATE TABLE [dbo].[Feeds]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Feeds2Tags]') AND type in (N'U'))
TRUNCATE TABLE [dbo].[FeedsToTags]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FeedsContent]') AND type in (N'U'))
TRUNCATE TABLE [dbo].[FeedsContent]
GO
