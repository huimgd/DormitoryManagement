
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/29/2019 14:58:55
-- Generated from EDMX file: D:\微软云下载内容\OneDrive - OBA GG\C#\寝室管理系统\DormitoryManagement\DormitoryManagement\Models\DataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DormitoryManagement];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_ClassClassRecord]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ClassRecord] DROP CONSTRAINT [FK_ClassClassRecord];
GO
IF OBJECT_ID(N'[dbo].[FK_StudentStudentRecord]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StudentRecord] DROP CONSTRAINT [FK_StudentStudentRecord];
GO
IF OBJECT_ID(N'[dbo].[FK_DormitoryInfoStudentRecord]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StudentRecord] DROP CONSTRAINT [FK_DormitoryInfoStudentRecord];
GO
IF OBJECT_ID(N'[dbo].[FK_StudentClassRecord]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ClassRecord] DROP CONSTRAINT [FK_StudentClassRecord];
GO
IF OBJECT_ID(N'[dbo].[FK_DetailsFloorDetailsLayer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DetailsLayer] DROP CONSTRAINT [FK_DetailsFloorDetailsLayer];
GO
IF OBJECT_ID(N'[dbo].[FK_DetailsLayerDormitoryInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Dormitory] DROP CONSTRAINT [FK_DetailsLayerDormitoryInfo];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Dormitory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Dormitory];
GO
IF OBJECT_ID(N'[dbo].[Student]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Student];
GO
IF OBJECT_ID(N'[dbo].[StudentRecord]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StudentRecord];
GO
IF OBJECT_ID(N'[dbo].[Class]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Class];
GO
IF OBJECT_ID(N'[dbo].[ClassRecord]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClassRecord];
GO
IF OBJECT_ID(N'[dbo].[DetailsFloor]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DetailsFloor];
GO
IF OBJECT_ID(N'[dbo].[DetailsLayer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DetailsLayer];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Dormitory'
CREATE TABLE [dbo].[Dormitory] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CreationTime] datetime  NOT NULL,
    [State] int  NOT NULL,
    [Number] nvarchar(max)  NOT NULL,
    [Capacity] int  NOT NULL,
    [Already] int  NOT NULL,
    [DetailsLayerId] int  NOT NULL
);
GO

-- Creating table 'Student'
CREATE TABLE [dbo].[Student] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Contact] nvarchar(max)  NULL,
    [State] int  NOT NULL
);
GO

-- Creating table 'StudentRecord'
CREATE TABLE [dbo].[StudentRecord] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CheckinTime] datetime  NULL,
    [DepartureTime] datetime  NULL,
    [StudentId] int  NOT NULL,
    [DormitoryInfoId] int  NOT NULL
);
GO

-- Creating table 'Class'
CREATE TABLE [dbo].[Class] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ClassName] nvarchar(max)  NOT NULL,
    [State] int  NOT NULL
);
GO

-- Creating table 'ClassRecord'
CREATE TABLE [dbo].[ClassRecord] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CheckinTime] datetime  NULL,
    [DepartureTime] datetime  NULL,
    [ClassId] int  NOT NULL,
    [StudentId] int  NOT NULL
);
GO

-- Creating table 'DetailsFloor'
CREATE TABLE [dbo].[DetailsFloor] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FloorName] nvarchar(max)  NOT NULL,
    [State] int  NOT NULL
);
GO

-- Creating table 'DetailsLayer'
CREATE TABLE [dbo].[DetailsLayer] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LayerNumber] nvarchar(max)  NOT NULL,
    [DetailsFloorId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Dormitory'
ALTER TABLE [dbo].[Dormitory]
ADD CONSTRAINT [PK_Dormitory]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Student'
ALTER TABLE [dbo].[Student]
ADD CONSTRAINT [PK_Student]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'StudentRecord'
ALTER TABLE [dbo].[StudentRecord]
ADD CONSTRAINT [PK_StudentRecord]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Class'
ALTER TABLE [dbo].[Class]
ADD CONSTRAINT [PK_Class]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ClassRecord'
ALTER TABLE [dbo].[ClassRecord]
ADD CONSTRAINT [PK_ClassRecord]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DetailsFloor'
ALTER TABLE [dbo].[DetailsFloor]
ADD CONSTRAINT [PK_DetailsFloor]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DetailsLayer'
ALTER TABLE [dbo].[DetailsLayer]
ADD CONSTRAINT [PK_DetailsLayer]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ClassId] in table 'ClassRecord'
ALTER TABLE [dbo].[ClassRecord]
ADD CONSTRAINT [FK_ClassClassRecord]
    FOREIGN KEY ([ClassId])
    REFERENCES [dbo].[Class]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ClassClassRecord'
CREATE INDEX [IX_FK_ClassClassRecord]
ON [dbo].[ClassRecord]
    ([ClassId]);
GO

-- Creating foreign key on [StudentId] in table 'StudentRecord'
ALTER TABLE [dbo].[StudentRecord]
ADD CONSTRAINT [FK_StudentStudentRecord]
    FOREIGN KEY ([StudentId])
    REFERENCES [dbo].[Student]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StudentStudentRecord'
CREATE INDEX [IX_FK_StudentStudentRecord]
ON [dbo].[StudentRecord]
    ([StudentId]);
GO

-- Creating foreign key on [DormitoryInfoId] in table 'StudentRecord'
ALTER TABLE [dbo].[StudentRecord]
ADD CONSTRAINT [FK_DormitoryInfoStudentRecord]
    FOREIGN KEY ([DormitoryInfoId])
    REFERENCES [dbo].[Dormitory]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DormitoryInfoStudentRecord'
CREATE INDEX [IX_FK_DormitoryInfoStudentRecord]
ON [dbo].[StudentRecord]
    ([DormitoryInfoId]);
GO

-- Creating foreign key on [StudentId] in table 'ClassRecord'
ALTER TABLE [dbo].[ClassRecord]
ADD CONSTRAINT [FK_StudentClassRecord]
    FOREIGN KEY ([StudentId])
    REFERENCES [dbo].[Student]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StudentClassRecord'
CREATE INDEX [IX_FK_StudentClassRecord]
ON [dbo].[ClassRecord]
    ([StudentId]);
GO

-- Creating foreign key on [DetailsFloorId] in table 'DetailsLayer'
ALTER TABLE [dbo].[DetailsLayer]
ADD CONSTRAINT [FK_DetailsFloorDetailsLayer]
    FOREIGN KEY ([DetailsFloorId])
    REFERENCES [dbo].[DetailsFloor]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetailsFloorDetailsLayer'
CREATE INDEX [IX_FK_DetailsFloorDetailsLayer]
ON [dbo].[DetailsLayer]
    ([DetailsFloorId]);
GO

-- Creating foreign key on [DetailsLayerId] in table 'Dormitory'
ALTER TABLE [dbo].[Dormitory]
ADD CONSTRAINT [FK_DetailsLayerDormitoryInfo]
    FOREIGN KEY ([DetailsLayerId])
    REFERENCES [dbo].[DetailsLayer]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetailsLayerDormitoryInfo'
CREATE INDEX [IX_FK_DetailsLayerDormitoryInfo]
ON [dbo].[Dormitory]
    ([DetailsLayerId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------