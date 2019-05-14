DROP TABLE IF EXISTS [dbo].[SeatfrogServices]
GO

DROP TABLE IF EXISTS [dbo].[SeatfrogLocations]
GO

DROP TABLE IF EXISTS [dbo].[SeatfrogServiceLocations]
GO

CREATE TABLE [dbo].[SeatfrogServices](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RetailServiceId] [char](8) NULL,
	[HeadCode] [char](4) NULL,
	CONSTRAINT PK_SeatfrogServices PRIMARY KEY ([Id])
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SeatfrogLocations](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[TipLoc] [varchar](7) NOT NULL,
	[ThreeLetterCode] [char](3) NULL
	CONSTRAINT PK_SeatfrogLocations PRIMARY KEY ([Id])
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SeatfrogServiceLocations](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ScheduleId] [bigint] NOT NULL,
	[LocationId] [bigint] NOT NULL,
	[Sequence] [int] NOT NULL,
	[PublicTime] [time] NULL,
	CONSTRAINT PK_SeatfrogServiceLocations PRIMARY KEY ([ScheduleId], [Id])
) ON [PRIMARY]
GO