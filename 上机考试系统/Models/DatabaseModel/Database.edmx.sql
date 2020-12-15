
SET QUOTED_IDENTIFIER OFF;
GO
USE [Database];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO



IF OBJECT_ID(N'[dbo].[student]', 'U') IS NOT NULL
    DROP TABLE [dbo].[student];
GO
IF OBJECT_ID(N'[dbo].[teacher]', 'U') IS NOT NULL
    DROP TABLE [dbo].[teacher];
GO


CREATE TABLE [dbo].[student] (
    [Id] int  NOT NULL,
    [name] nvarchar(10)  NULL,
    [ip_address] varchar(15)  NULL,
    [pwd] varchar(20)  NOT NULL
);
GO


CREATE TABLE [dbo].[teacher] (
    [Id] int  NOT NULL,
    [name] nvarchar(10)  NULL,
    [pwd] varchar(50)  NOT NULL
);
GO


ALTER TABLE [dbo].[student]
ADD CONSTRAINT [PK_student]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

ALTER TABLE [dbo].[teacher]
ADD CONSTRAINT [PK_teacher]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO
