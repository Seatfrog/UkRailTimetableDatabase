select *
from  stations;

select s.ThreeLetterCode, s.TipLoc, s.Description, count(*)
from Associations a
inner join Locations l on a.LocationId = l.Id
inner join TimetableRdg.dbo.stations s on l.TipLoc = s.TipLoc
group by s.ThreeLetterCode, s.TipLoc, s.Description
order by s.ThreeLetterCode;

select s.ThreeLetterCode, s.TipLoc, s.Description, a.*
from Associations a
inner join Locations l on a.LocationId = l.Id
inner join TimetableRdg.dbo.stations s on l.TipLoc = s.TipLoc
where l.tiploc = 'PADTON'
	and (a.MainUid in ('C59088','C59038') or a.AssociatedUid in ('C59088','C59038'))
order by s.ThreeLetterCode, a.id;

select *
from dbo.Schedules s
where s.TimetableUid in ('C59088','C59038', 'C58006', 'C57751');

select s.TimetableUid, l.Description, l.TipLoc, l.ThreeLetterCode, sl.*
from dbo.Schedules s
inner join dbo.ScheduleLocations sl on s.id = sl.ScheduleId
inner join dbo.Locations l on l.id = sl.LocationId
where s.TimetableUid in ('C59088','C59038', 'C58006', 'C57751') and s.StpIndicator = 'P'
order by sl.id;

select s.TimetableUid, l.Description, l.TipLoc, l.ThreeLetterCode, sl.*
from dbo.Schedules s
inner join dbo.ScheduleLocations sl on s.id = sl.ScheduleId
inner join dbo.Locations l on l.id = sl.LocationId
where s.Id in (46814)
order by sl.id;
