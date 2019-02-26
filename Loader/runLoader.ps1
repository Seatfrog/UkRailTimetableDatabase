param(
[string] $cifFile    # National Rail Open Data CIF file to
)

# Create database, uncomment if neccessary.
# Invoke-Sqlcmd -Database -InputFile .\Database\Scripts\CreateDatabase.sql

Write-Host "Create tables"
Invoke-Sqlcmd -Database Timetable -InputFile "..\Database\Scripts\CreateSchema.sql"

Write-Host "Load $cifFile"
Start-Process -FilePath 'dotnet' -Wait -ArgumentList ".\TimetableLoader\bin\Debug\netcoreapp2.2\TimetableLoader.dll -i $cifFile"

Write-Host "Add indices"
Invoke-Sqlcmd -Database Timetable -InputFile ..\Database\Scripts\CreateIndices.sql