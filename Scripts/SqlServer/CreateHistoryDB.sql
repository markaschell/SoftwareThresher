USE [master]
GO

-- Create history Database
CREATE DATABASE [history]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'History', FILENAME = N'<<ENTER LOCATION HERE>>\Performance.mdf', SIZE = 1502848KB, MAXSIZE = UNLIMITED, FILEGROWTH = 100MB )
 LOG ON 
( NAME = N'History_log', FILENAME = N'<<ENTER LOCATION HERE>>\HISTORY\History_log.ldf', SIZE = 1MB, MAXSIZE = 2GB, FILEGROWTH = 100MB )
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [history].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO

-- Update these per the defaults after creating one
ALTER DATABASE [history] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [history] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [history] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [history] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [history] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [history] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [history] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [history] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [history] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [history] SET DISABLE_BROKER 
GO

ALTER DATABASE [history] SET AUTO_UPDATE_STATISTICS_ASYNC ON 
GO

ALTER DATABASE [history] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [history] SET PAGE_VERIFY CHECKSUM  
GO



-- Create performance_stats table
USE [history]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[performance_stats](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[database_name] [sysname] NOT NULL,
	[object_name] [sysname] NOT NULL,
	[plan_handle] [varbinary](64) NULL,
	[cached_time] [datetime] NULL,
	[last_execution_time] [datetime] NULL,
	[execution_count] [bigint] NULL,
 CONSTRAINT [PK_performance_stats] PRIMARY KEY NONCLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



-- Create UpdatePerformanceStats Stored Procedure 
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdatePerformanceStats]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
     DROP PROCEDURE [dbo].UpdatePerformanceStats
GO

CREATE PROCEDURE [dbo].[UpdatePerformanceStats]
AS
BEGIN
     MERGE history.dbo.performance_stats ps
     USING (
         SELECT DB_NAME(database_id) database_name, OBJECT_NAME(object_id, database_id) procedure_name, plan_handle, cached_time, last_execution_time, execution_count
         FROM sys.dm_exec_procedure_stats
         WHERE OBJECT_NAME(object_id, database_id) NOT LIKE 'MSmerge%'
         AND DB_NAME(database_id) NOT IN ('msdb', 'master', 'model', 'tempdb')
         AND DB_NAME(database_id) IS NOT NULL
     ) AS dmv
     ON ps.plan_handle = dmv.plan_handle AND ps.cached_time = dmv.cached_time
     WHEN MATCHED THEN
         UPDATE SET
             ps.last_execution_time = dmv.last_execution_time,
             ps.execution_count = dmv.execution_count
     WHEN NOT MATCHED
     THEN INSERT VALUES
         (dmv.database_name, dmv.procedure_name, dmv.plan_handle, dmv.cached_time, dmv.last_execution_time, dmv.execution_count);
END
 
GO

SET ANSI_PADDING OFF
GO



-- Set permissions on performance_stats?
-- setup a job to run UpdatePerformanceStats every day
-- Setup a job to clean up performance_stats
--   DELETE FROM performance_stats WHERE last_execution_time < dateadd(d,  365 * 2.5, getdate())