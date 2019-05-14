select s.TimetableUid, s.StpIndicator, s.DayMask, s.RunsFrom, s.RunsTo
from schedules s
where s.toc = 'CS' and s.StpIndicator = 'P'
order by s.TimetableUid, s.StpIndicator;

select s.TimetableUid, l.*, sl.*
from schedules s
inner join ScheduleLocations sl on s.Id = sl.ScheduleId
inner join Locations l on l.id = sl.LocationId
where s.toc = 'SW' and s.StpIndicator = 'P' and l.ThreeLetterCode = 'SUR'
order by sl.PublicArrival, sl.ScheduleId, sl.id;

select s.TimetableUid, s.RunsFrom, s.RunsTo, s.DayMask, l.ThreeLetterCode, sl.*
from schedules s
inner join ScheduleLocations sl on s.Id = sl.ScheduleId
inner join Locations l on l.id = sl.LocationId
where s.TimetableUid IN ('W90151', 'W90152', 'W90153', 'W90154', 'W33189') and l.ThreeLetterCode = 'SUR' -- G63001  G61085
order by s.TimetableUid, s.RunsFrom, sl.id;

select s.TimetableUid, s.RunsFrom, s.RunsTo, s.DayMask, l.ThreeLetterCode, sl.*
from schedules s
inner join ScheduleLocations sl on s.Id = sl.ScheduleId
inner join Locations l on l.id = sl.LocationId
where s.TimetableUid IN ('G61069')
order by s.TimetableUid, s.RunsFrom, sl.id;

select s.RetailServiceId, s.TimetableUid, s.StpIndicator, s.RunsFrom, s.RunsTo, s.DayMask, l.ThreeLetterCode, l.Description, sl.*
from schedules s
inner join ScheduleLocations sl on s.Id = sl.ScheduleId
inner join Locations l on l.id = sl.LocationId
where sl.Activities like 'AE%' --  s.TimetableUid IN ('W33239', 'Q53671', 'Q53672')
order by s.TimetableUid, s.RunsFrom, sl.id;

select s.*
from schedules s
where s.TimetableUid IN ('W90151', 'W90152', 'W90153', 'W90154', 'W33189')
order by s.TimetableUid, s.RunsFrom, s.StpIndicator desc;

select MIN(s.RunsFrom), MAX(s.RunsTo)
from schedules s;

select *
from Stations s
where s.ThreeLetterCode = 'CLJ';

select l.*, s.*
from Locations l
inner join Stations s on s.TipLoc = l.TipLoc
where s.ThreeLetterCode = 'CLJ';

select s.timetableuid, count(distinct s.toc)
from schedules s
where s.StpIndicator IN ('P', 'N')
group by s.timetableuid
having count(distinct s.toc) > 1
order by s.TimetableUid;

select s.*
from schedules s
inner join (
select s.timetableuid
from schedules s
where s.StpIndicator = 'P'
group by s.timetableuid
having count(distinct s.toc) > 1) dups ON s.TimetableUid = dups.TimetableUid and s.StpIndicator = 'P'
order by s.TimetableUid, s.RunsFrom;

select s.PortionId, s.toc, COUNT(*)
from Schedules s
where s.PortionId = 'Z'
group by s.PortionId, s.toc;

select count(*)
from Schedules s;

select s.schedules, count(*)
from 
(select s.TimetableUid, count(*)as schedules
from Schedules s
group by s.TimetableUid) s
group by s.schedules
order by s.schedules;;

select s.stops, count(*)
from 
(select count(*) as stops
from ScheduleLocations sl
inner join Locations l on l.id = sl.LocationId
inner join Stations sn on sn.tiploc = l.TipLoc
where not sl.PublicDeparture is null
group by sl.ScheduleId) s
group by s.stops
order by s.stops;

select s.stops, count(*)
from 
(select count(*) as stops
from ScheduleLocations sl
inner join Locations l on l.id = sl.LocationId
inner join Stations sn on sn.tiploc = l.TipLoc
where not sl.PublicArrival is null
group by sl.LocationId) s
group by s.stops
order by s.stops;

select top 100 *
from schedules s
where s.StpIndicator = 'C' and s.TimetableUid like 'W%';

select *
from Schedules s
where s.TimetableUid in ('L28873', 'L27215', 'W93794', 'W01129');

select s.StpIndicator, s.status, s.Category, count(*)
from Schedules s
group by s.status, s.Category, s.StpIndicator
order by s.status, s.Category;

select s.Activities, count(*)
from ScheduleLocations s
group by s.Activities
order by s.Activities;

select s.Activities, count(*)
from ScheduleLocations s
where (not s.PublicArrival is null) OR (not s.PublicDeparture is null)
group by s.Activities
order by s.Activities;

select s.*
from schedules s
where s.TimetableUid IN ('W90200', 'F01470', 'J61218')
order by s.TimetableUid, s.RunsFrom, s.StpIndicator desc;

select s.RetailServiceId, s.TimetableUid, s.StpIndicator, s.RunsFrom, s.RunsTo, s.DayMask, s.TrainIdentity, s.Toc, count(*)
from schedules s
inner join ScheduleLocations sl on s.Id = sl.ScheduleId
where s.RetailServiceId IN ('CC075900', 'LO272300', 'SE214500', 'EM744100', 'SW917500')
	or s.TimetableUid IN ('W90200', 'F01470', 'J61218')
group by s.RetailServiceId, s.TimetableUid, s.StpIndicator, s.RunsFrom, s.RunsTo, s.DayMask, s.TrainIdentity, s.Toc
order by s.RetailServiceId, s.RunsFrom, s.StpIndicator desc, s.TimetableUid;

select s.*
from Associations s
where s.AssociatedUid IN ('Q53671', 'Q53672') or s.MainUid IN ('Q53671', 'Q53672')
order by s.RunsFrom, s.StpIndicator desc;