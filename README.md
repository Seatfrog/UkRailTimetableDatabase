# UK Rail Timetable Database + loader
A SQL Server database schema to load a UK timetable into plus a .Net Core app to load the data.  The loader reads a full CIF timetable file into the database

## How do I load a CIF file?

The `runLoader.ps1` script will create the schema (deleting any existing tables and data), load a timetable and then add some indices to the database.

It assumes its connecting to an existing local database using integrated security and the database is called Timetable.
`CreateTimetable.sql` will create the physical database in the default location.  `CreateTimetableRdg.sql` creates a separate database called TimetableRdg should you want to have both.

`runLoader.ps1` and `Loader/Timetable/appSettings.json` need to be updated with any different connection details 

* `runLoadFullNrodFile.bat` shows how to call `runloader.ps1` for a Network Rail cif file.
* `runLoadFullRdgFile.bat` shows how to call `runloader.ps1` for a RDG cif file.

## Development Dependencies

The solution `TimetableLoader.sln` contains 6 projects, the 2 in this repo plus those from the CifParser repo.  It assumes that the CifParser repo has been cloned as a sibling of the `UkRailTimetableDatabase` repo.  At some point I will look to change this to either reference Nuget packages or do it as a git submodule.

## Limitations

Currently the loader has assumptions that it is reading in a full CIF in the format supplied by National Rail Open Data (NROD) or RDG.  In the RDG case it loads the CIF file plus the Master Station file.

There are assumptions built into the loader that imply loading daily deltas files is problematic (it generates Ids during load, would need to select out of the database).  I don't intend to implement this functionality given a full file is available everyday from NROD.


## Database Ids = cif file line number

Due to the way database Ids are generated it created a quite nice unintended consequence that the Id is actually the line number of the record in the cif file.  There is no guarantee this will always be the case.


