bcp TimetableRdg.dbo.SeatfrogLocations IN C:\Users\phils\OneDrive\Data\Timetable\Seatfrog\locations.csv -f C:\Users\phils\source\repos\UkRailTimetableDatabase\Database\SeatfrogLocations.xml -T -E -F 2 -e errorLocations.txt
bcp TimetableRdg.dbo.SeatfrogServices IN C:\Users\phils\OneDrive\Data\Timetable\Seatfrog\rsid.csv -f C:\Users\phils\source\repos\UkRailTimetableDatabase\Database\SeatfrogServices.xml -T -E -F 2 -e errorServices.txt
bcp TimetableRdg.dbo.SeatfrogServiceLocations IN C:\Users\phils\OneDrive\Data\Timetable\Seatfrog\stops.csv -f C:\Users\phils\source\repos\UkRailTimetableDatabase\Database\SeatfrogServiceLocations.xml -T -E -F 2 -e errorStops.txt