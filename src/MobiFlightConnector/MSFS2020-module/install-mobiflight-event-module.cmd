setLocal EnableDelayedExpansion
@echo off
IF EXIST "%LOCALAPPDATA%\Packages\Microsoft.FlightSimulator_8wekyb3d8bbwe\LocalCache\UserCfg.opt" (
	echo store version
	SET TOSEARCH="%LOCALAPPDATA%\Packages\Microsoft.FlightSimulator_8wekyb3d8bbwe\LocalCache\UserCfg.opt"
) ELSE (
	echo normal version
	SET TOSEARCH="%appdata%\Microsoft Flight Simulator\UserCfg.opt"
)
IF EXIST %TOSEARCH% (
	for /f "delims=" %%i in ('findstr "InstalledPackagesPath " %TOSEARCH%') do (
		SET RESULT=%%i
	)
	echo found !RESULT!
	SET RESULT=!RESULT:~23!
	SET RESULT=!RESULT:~0,-1!
	echo transform !RESULT!
	IF EXIST !RESULT!\Community\ (
		xcopy /F /R /Y /E /C .\mobiflight-event-module !RESULT!\Community\mobiflight-event-module\
		exit 0
	) ELSE (
		echo ERROR Community folder not found...
	)
) ELSE (
	echo ERROR MSFS2020 config file not found...
)
pause
exit 0

