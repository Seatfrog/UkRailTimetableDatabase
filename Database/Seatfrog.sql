-- VT656000 for tomorrow, that should be the 0630 GLC EUS but it is showing as departing at 0730 in the dashboard. 
-- Similarly the VT604000 departs at 0830 but the dashboard shows 0930. 

select *
from schedules s
where s.RetailServiceId in ('VT656000', 'VT604000')
order by s.RetailServiceId, s.StpIndicator, s.RunsFrom;

select s.RetailServiceId, s.TimetableUid, s.StpIndicator, s.RunsFrom, s.RunsTo, s.DayMask, l.ThreeLetterCode, sl.*
from schedules s
inner join ScheduleLocations sl on s.Id = sl.ScheduleId
inner join Locations l on l.id = sl.LocationId
where s.TimetableUid in ('Y81839', 'Y81738')
order by s.RetailServiceId, s.StpIndicator, sl.id;

select f.*, s.Id, s.RetailServiceId, s.TimetableUid, s.StpIndicator, s.RunsFrom, s.RunsTo, s.TrainIdentity
from dbo.SeatfrogServices f
inner join dbo.Schedules s on s.RetailServiceId = f.RetailServiceId
where not s.TrainIdentity = f.HeadCode
order by f.headcode, s.TimetableUid, s.RunsFrom, s.StpIndicator;

select fs.*, s.Id, s.RetailServiceId, s.TimetableUid, s.StpIndicator, s.RunsFrom, s.RunsTo, s.TrainIdentity, fsl.*, fl.*, l.Tiploc, l.ThreeLetterCode  
from dbo.SeatfrogServices fs
inner join dbo.Schedules s on s.RetailServiceId = fs.RetailServiceId
inner join dbo.SeatfrogServiceLocations fsl on fs.Id = fsl.ScheduleId
inner join dbo.SeatfrogLocations fl on fl.Id = fsl.LocationId
inner join dbo.Stations st on fl.Tiploc = l.TipLoc
inner join dbo.Locations l on fl.Tiploc = l.TipLoc
where s.TimetableUid in ('Y81839', 'Y81738')
order by s.TimetableUid, s.RunsFrom, s.StpIndicator;