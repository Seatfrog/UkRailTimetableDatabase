use Timetable;

select l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode, count(*)
from locations l
inner join schedulelocations sl on l.id = sl.locationid
where l.ThreeLetterCode = 'WAT'
or l.nlc like '5598%'
group by l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode
UNION 
select l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode, 0
from locations l
where l.ThreeLetterCode = 'WAT'
or l.nlc like '5598%';

select l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode, count(*)
from locations l
inner join schedulelocations sl on l.id = sl.locationid
where l.ThreeLetterCode = 'SUR'
or l.nlc like '5571%'
group by l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode;

select *
from schedulelocations sl
inner join locations l on l.id = sl.locationid
where sl.scheduleid < 25;

select count(*), l.id, l.TipLoc, l.nlc, l.Stanox, l.ThreeLetterCode, l.Description
from schedulelocations sl
inner join locations l on l.id = sl.locationid
group by l.id, l.TipLoc, l.nlc, l.Stanox, l.ThreeLetterCode, l.Description
order by l.nlc;

select s.id, s.TimetableUid, s.StpIndicator, s.runsfrom, s.RunsTo, s.DayMask, s.toc, l.tiploc, l.description, l.nlc, l.threelettercode, sl.*
from schedules s 
inner join schedulelocations sl on sl.scheduleId = s.id
inner join locations l on l.id = sl.locationid
left outer join schedulechanges c on s.id = c.scheduleid 
where s.TimetableUid IN ('C10037', 'C10188')
order by sl.id;


select top 10 *
from associations;
