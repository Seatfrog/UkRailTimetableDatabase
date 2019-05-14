
select s.StpIndicator, s.status, s.Category, count(*)
from Schedules s
group by s.status, s.Category, s.StpIndicator
order by s.status, s.Category;

select s.status, s.Category, count(*)
from Schedules s
group by s.status, s.Category
order by s.status, s.Category;

select s.Category, count(*)
from Schedules s
group by s.Category
order by s.Category;

select s.*
from schedules s
where s.TimetableUid IN ('W90200', 'F01470', 'J61218')
order by s.TimetableUid, s.RunsFrom, s.StpIndicator desc;