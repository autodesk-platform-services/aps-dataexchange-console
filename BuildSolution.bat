@ECHO OFF
ECHO =======================
ECHO Console Connector Build
ECHO =======================

REM Clearing all logs if the folder exist
IF EXIST "buildlogs" (
cd "buildlogs"
del *.log /F /Q
    if EXIST "warnings" (
        cd "warnings"
        del *.log /F /Q
        cd..
    )
cd..
)

set "installedPackgesFolder=./packages"

REM Delete installed packages folder for a clean restore
if exist "%installedPackgesFolder%" rmdir /s /q "%installedPackgesFolder%"

REM Please ensure msbuild variables are added as Path environment variables or run the batch script in developer command promt

ECHO restore nuget pacakges
msbuild -t:restore -p:RestorePackagesConfig=true ConsoleConnector.sln

ECHO msbuild
msbuild ConsoleConnector.sln /p:Configuration=Debug /p:Platform="x64" -flp1:logfile=./buildlogs/errors.log;errorsonly -flp2:logfile=./buildlogs/warnings.log;warningsonly

ECHO ConsoleConnector.sln build done!
ECHO You can find warnings and errors in the buildlogs folder



PAUSE
