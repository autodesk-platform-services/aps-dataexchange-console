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

for %%I in ("%~dp0..\") do set "ParentDir=%%~fI"

REM Specify the path to nuget pakages folder
set "targetFolder=%ParentDir%"
set "installedPackgesFolder=./packages"

REM REM Delete installed packages folder
if exist "%installedPackgesFolder%" rmdir /s /q "%installedPackgesFolder%"

(
  echo ^<configuration^>
  echo   ^<packageSources^>
  echo     ^<add key="NuGetOrg" value="https://api.nuget.org/v3/index.json" /^>
  echo     ^<add key="TempSource" value="%targetFolder%" /^>
  echo   ^</packageSources^>
  echo ^</configuration^>
) > NuGet.config


REM Please ensure msbuild variables are added as Path environment variables or run the batch script in developer command promt

ECHO restore nuget pacakges
msbuild -t:restore -p:RestorePackagesConfig=true ConsoleConnector.sln /p:NugetConfig=./NuGet.config 

ECHO msbuild
msbuild ConsoleConnector.sln /p:Configuration=Debug /p:Platform="x64" -flp1:logfile=./buildlogs/errors.log;errorsonly -flp2:logfile=./buildlogs/warnings.log;warningsonly

if exist NuGet.config del Nuget.config

ECHO ConsoleConnector.sln build done!
ECHO You can find warnings and errors in the buildlogs folder



PAUSE
