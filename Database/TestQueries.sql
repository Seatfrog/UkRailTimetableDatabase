use Timetable;

select *
from locations;

select *
from schedules s
where s.id < 25;

select *
from schedulelocations sl
inner join locations l on l.id = sl.locationid
where sl.scheduleid < 25;

select count(*), l.id, l.TipLoc, l.Description
from schedulelocations sl
inner join locations l on l.id = sl.locationid
group by l.id, l.TipLoc, l.Description
order by l.tiploc;

select l.id, l.TipLoc, l.Description, s.id, s.TimetableUid, sl.id, sl.scheduleid, c.*
from schedulechanges c
inner join schedules s on s.id = c.scheduleid 
inner join schedulelocations sl on c.schedulelocationId = sl.id
inner join locations l on l.id = sl.locationid
where s.TimetableUid = 'U49566'
order by c.id;


select *
from schedules s
where (NOT s.Action = 'D')
	AND (NOT s.StpIndicator = 'C')
	AND (s.Toc is null or s.Toc = '');