use Timetable;

select distinct l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode, l.Description
from locations l
left outer join schedulelocations sl on l.id = sl.locationid
where sl.locationid is null and not l.ThreeLetterCode is null
order by l.ThreeLetterCode;


select l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode, count(*)
from locations l
inner join schedulelocations sl on l.id = sl.locationid
where l.ThreeLetterCode = 'SUR'
or l.nlc like '5571%'
group by l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode;

select stationId, stationCrs, stationName, stationTiploc, id, tiploc, nlc, Stanox, ThreeLetterCode, Description, sum(stops)
from (
	select s.Id as stationId, s.ThreeLetterCode as stationCrs, s.Description as stationName, s.TipLoc as stationTiploc, 
	l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode, l.Description, count(*) stops
	from stations s
	inner join locations l on s.Tiploc = l.TipLoc
	inner join schedulelocations sl on l.id = sl.locationid
	where l.ThreeLetterCode = 'CLJ'
	or l.nlc like '5595%' or l.Stanox = '87219' or l.Description like '%CLAPHAM J%'
	group by s.Id, s.ThreeLetterCode, s.Description, s.TipLoc, l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode, l.Description
	UNION 
	select s.Id, s.ThreeLetterCode, s.Description, s.TipLoc, l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode, l.Description, 0
	from stations s
	inner join locations l on s.Tiploc = l.TipLoc
	where l.ThreeLetterCode = 'CLJ'
	or l.nlc like '5595%' or l.Stanox = '87219' or l.Description like '%CLAPHAM J%') l
group by stationId, stationCrs, stationName, stationTiploc, id, tiploc, nlc, Stanox, ThreeLetterCode, Description
order by nlc;

select stationId, stationCrs, stationName, stationTiploc, id, tiploc, nlc, Stanox, ThreeLetterCode, Description, sum(stops)
from (
	select s.Id as stationId, s.ThreeLetterCode as stationCrs, s.Description as stationName, s.TipLoc as stationTiploc, 
	l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode, l.Description, count(*) stops
	from stations s
	inner join locations l on s.Tiploc = l.TipLoc
	inner join schedulelocations sl on l.id = sl.locationid
	where s.ThreeLetterCode = 'LTV'
	or l.nlc like '1291%' or l.nlc like '1292%'	-- Lichfield Trent valley
	group by s.Id, s.ThreeLetterCode, s.Description, s.TipLoc, l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode, l.Description
	UNION 
	select s.Id, s.ThreeLetterCode, s.Description, s.TipLoc, l.id, l.tiploc, l.nlc, l.Stanox, l.ThreeLetterCode, l.Description, 0
	from stations s
	inner join locations l on s.Tiploc = l.TipLoc
	where s.ThreeLetterCode = 'LTV'
	or l.nlc like '1291%' or l.nlc like '1292%') l
group by stationId, stationCrs, stationName, stationTiploc, id, tiploc, nlc, Stanox, ThreeLetterCode, Description
order by nlc;


select *
from locations l
where description like '%LICH%' or ThreeLetterCode = 'LTV';

select top 100 *
from schedulelocations sl
where sl.locationid in (9419, 9420) ; -- 9405, 

select count(*), l.id, l.TipLoc, l.nlc, l.Stanox, l.ThreeLetterCode, l.Description
from schedulelocations sl
inner join locations l on l.id = sl.locationid
group by l.id, l.TipLoc, l.nlc, l.Stanox, l.ThreeLetterCode, l.Description
order by l.nlc;

select s.id, s.TimetableUid, s.RetailServiceId, s.StpIndicator, s.runsfrom, s.RunsTo, s.DayMask, s.toc, l.tiploc, l.description, l.nlc, l.threelettercode, sl.*
from schedules s 
inner join schedulelocations sl on sl.scheduleId = s.id
inner join locations l on l.id = sl.locationid
where  s.TimetableUid IN ('C60436', 'C60602', 'C61577') --  s.id IN (281856, 282019, 433574, 433429) s.TimetableUid IN ('C60436', 'C61577')
order by sl.id;

select s.toc, s.Status, s.Category, count(*)
from Schedules s
group by s.toc, s.Status, s.Category
order by s.toc, s.Status, s.Category;

select s.Status, s.StpIndicator, count(*)
from Schedules s
group by s.Status, s.StpIndicator
order by s.StpIndicator desc, s.Status;

select top 10 *
from schedules s
where s.status is null;

select top 10 *
from associations a
inner join locations l on l.Id = a.LocationId;

select s.tiploc, s.ThreeLetterCode, s.SubsidiaryThreeLetterCode, s.Description, l.tiploc, l.ThreeLetterCode, l.Description
from stations s
full join locations l on l.TipLoc = s.TipLoc
where l.tiploc is null and not (s.Description like '%CIE%' OR s.Description like '%MTLK%')
-- where s.TipLoc is null or l.tiploc is null
order by s.Description, l.Description;

select s.tiploc, s.ThreeLetterCode, s.SubsidiaryThreeLetterCode, s.Description, l.tiploc, l.ThreeLetterCode, l.Description
from stations s
inner join locations l on l.TipLoc = s.TipLoc
where l.ThreeLetterCode <> s.ThreeLetterCode
-- where s.TipLoc is null or l.tiploc is null
order by s.Description, l.Description;

select s.tiploc, s.ThreeLetterCode, s.SubsidiaryThreeLetterCode, s.Description, l.tiploc, l.ThreeLetterCode, l.Description
from stations s
full join locations l on l.TipLoc = s.TipLoc
where l.ThreeLetterCode IN ('SCG', 'WIJ', 'CRE', 'SFA', 'TAM', 'WAT', 'LTV')  OR s.ThreeLetterCode IN ('SCG', 'WIJ', 'CRE', 'SFA', 'TAM', 'WAT', 'LTV')
-- where s.TipLoc is null or l.tiploc is null
order by s.Description, l.Description;