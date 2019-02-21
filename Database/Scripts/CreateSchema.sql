USE [Timetable]
GO

DROP TABLE IF EXISTS [dbo].[ScheduleLocations]
GO

DROP TABLE IF EXISTS [dbo].[ScheduleChanges]
GO

DROP TABLE IF EXISTS [dbo].[Schedules]
GO

DROP TABLE IF EXISTS [dbo].[Locations]
GO

USE [Timetable]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Locations](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
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
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Action] [char](1) NOT NULL,
	[StpIndicator] [char](1) NOT NULL,
	[TimetableUid] [char](6) NOT NULL,
	[RunsFrom] [date] NOT NULL,
	[RunsTo] [date] NULL,
	[DayMask] [tinyint] NULL,
	[BankHolidayRunning] [varchar](1) NULL,
	[Status] [char](1) NULL,
	[Category] [char](2) NULL,
	[TrainIdentity] [char](4) NULL,
	[NrsHeadCode] [varchar](4) NULL,
	[ServiceCode] [varchar](8) NULL,
	[PortionId] [varchar](1) NULL,
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

CREATE TABLE [dbo].[ScheduleLocations](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ScheduleId] [bigint] NOT NULL,
	[LocationId] [bigint] NOT NULL,
	[Sequence] [int] NOT NULL,
	[WorkingArrival] [time] NULL,
	[WorkingDeparture] [time] NULL,
	[WorkingPass] [time] NULL,
	[PublicArrival] [time] NULL,
	[PublicDeparture] [time] NULL,
	[Platform] [varchar](3) NULL,
	[Line] [varchar](3) NULL,
	[Path] [varchar](3) NULL,
	[Activities] [varchar](12) NULL,
	[EngineeringAllowance] [varchar](2) NULL,
	[PathingAllowance] [varchar](2) NULL,
	[PerformanceAllowance] [varchar](2) NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ScheduleChanges](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ScheduleId] [bigint] NOT NULL,
	[ScheduleLocationId] [bigint] NOT NULL,
	[Category] [char](2) NULL,
	[TrainIdentity] [char](4) NULL,
	[NrsHeadCode] [varchar](4) NULL,
	[ServiceCode] [varchar](8) NULL,
	[PortionId] [varchar](1) NULL,
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
	[RetailServiceId] [char](8) NULL
) ON [PRIMARY]
GO