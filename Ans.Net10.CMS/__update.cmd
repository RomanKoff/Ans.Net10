@echo off

rem set /p host="Enter host path: "
rem set /p site="Enter site url: "
rem set host=W:\site1
rem set site=https://site1
if "%host%"=="" goto END
echo.

echo !!! Warning!
echo !!! The core of the site '%host%' will be updated!
echo.
pause

echo Stopping the site application.
rename %host%\_app_offline.htm app_offline.htm
echo.
pause

echo Removing the old core of the site.
timeout /T 10
rd /s %host%\Areas
rd /s %host%\en
rd /s %host%\refs
rd /s %host%\runtimes
erase %host%\Ans.Net10*.xml
erase %host%\*.endpoints.json
erase %host%\*.runtimeconfig.json
erase %host%\*.deps.json
erase %host%\*.dll
erase %host%\*.pdb
erase %host%\*.exe
echo [OK] The old core of the site has been removed.
echo.
pause

echo Copying a new site core.
xcopy Areas %host%\Areas /s /e /i
xcopy en %host%\en /s /e /i
xcopy refs %host%\refs /s /e /i
xcopy runtimes %host%\runtimes /s /e /i
xcopy Ans.Net10*.xml %host% /i
xcopy *.endpoints.json %host% /i
xcopy *.runtimeconfig.json %host% /i
xcopy *.deps.json %host% /i
xcopy *.dll %host% /i
xcopy *.pdb %host% /i
xcopy *.exe %host% /i
erase /q %host%\__update-core.cmd
echo [OK] The new core has been copied
echo.

echo Launching the site application.
rename %host%\app_offline.htm _app_offline.htm
echo.

echo [END] The core of the site has been updated.
echo.
start %site%

:END

pause
