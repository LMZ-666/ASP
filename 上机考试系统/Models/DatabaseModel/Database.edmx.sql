
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/16/2020 17:31:51
-- Generated from EDMX file: C:\Users\User\source\repos\ASP\上机考试系统\Models\DatabaseModel\Database.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Database];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Exam]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Exam];
GO
IF OBJECT_ID(N'[dbo].[student]', 'U') IS NOT NULL
    DROP TABLE [dbo].[student];
GO
IF OBJECT_ID(N'[dbo].[teacher]', 'U') IS NOT NULL
    DROP TABLE [dbo].[teacher];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'student'
CREATE TABLE [dbo].[student] (
    [Id] int  NOT NULL,
    [name] nvarchar(10)  NULL,
    [ip_address] varchar(15)  NULL,
    [pwd] varchar(20)  NOT NULL
);
GO

-- Creating table 'teacher'
CREATE TABLE [dbo].[teacher] (
    [Id] int  NOT NULL,
    [name] nvarchar(10)  NULL,
    [pwd] varchar(50)  NOT NULL
);
GO

-- Creating table 'Exam'
CREATE TABLE [dbo].[Exam] (
    [Id] int  NOT NULL,
    [name] nvarchar(50)  NULL,
    [time] nvarchar(50)  NULL,
    [creatorId] int  NOT NULL,
    [test_upload] nvarchar(50)  NULL,
    [commmit_number] int  NULL,
    [is_being] nchar(1)  NULL,
    [has_saved] nchar(1)  NULL,
    [has_cleaned] nchar(1)  NULL,
    [creator] nchar(10)  NULL,
    [has_stopped] nchar(10)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'student'
ALTER TABLE [dbo].[student]
ADD CONSTRAINT [PK_student]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'teacher'
ALTER TABLE [dbo].[teacher]
ADD CONSTRAINT [PK_teacher]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Exam'
ALTER TABLE [dbo].[Exam]
ADD CONSTRAINT [PK_Exam]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------