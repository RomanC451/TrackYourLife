@echo off
REM The first parameter is the ModuleName
SET ModuleName=%1
REM The second parameter is the MigrationName
SET MigrationName=%2

REM Check if ModuleName is equal to Common
SET "ProjectPath=Modules\%ModuleName%\TrackYourLife.Modules.%ModuleName%.Infrastructure"


REM Construct the DbContext name
SET DbContextName=%ModuleName%WriteDbContext


dotnet ef database update %MigrationName% --startup-project App --project %ProjectPath% --context %DbContextName%
