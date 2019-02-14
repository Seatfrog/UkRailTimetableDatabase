USE [Timetable]
GO

DROP TABLE [dbo].[Schedules]
GO

DROP TABLE [dbo].[Locations]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Locations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Action] [char](1) NOT NULL,
	[TipLoc] [varchar](7) NOT NULL,
	[Description] [varchar](26) NULL,
	[Nlc] [char](6) NULL,
	[NlcCheckCharacter] [char](1) NULL,
	[NlcDescription] [varchar](16) NULL,
	[Stanox] [char](5) NULL,
	[ThreeLetterCode] [char](3) NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Schedules](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Action] [char](1) NOT NULL,
	[StpIndicator] [char](1) NOT NULL,
	[TimetableUid] [char](6) NOT NULL,
	[RunsFrom] [date] NOT NULL,
	[RunsTo] [date] NOT NULL,
	[DayMask] [tinyint] NOT NULL,
	[BankHolidayRunning] [varchar](1) NOT NULL,
	[Status] [char](1) NULL,
	[Category] [char](2) NULL,
	[TrainIdentity] [char](4) NULL,
	[NrsHeadCode] [varchar](4) NULL,
	[ServiceCode] [varchar](8) NULL,
	[PortiionId] [varchar](1) NULL,
	[PowerType] [char](3) NULL,
	[TimingLoadType] [varchar](4) NULL,
	[Speed] [int] NULL,
	[OperatingCharacteristics] [varchar](6) NULL,
	[SeatClass] [char](1) NULL,
	[SleeperClass] [varchar](1) NULL,
	[ReservationIndicator] [varchar](1) NULL,
	[Catering] [varchar](4) NULL,
	[Branding] [varchar](4) NULL,
	[EuropeanUic] [char](5) NULL,
	[Toc] [char](2) NULL,
	[ApplicableTimetable] [bit] NULL,
	[RetailServiceId] [char](8) NULL
) ON [PRIMARY]
GO

