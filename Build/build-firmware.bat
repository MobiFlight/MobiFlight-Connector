@ECHO OFF
SET SOLUTION_DIR=%1
SET RELEASE_DIR=%SOLUTION_DIR%\firmware
SET ICONFILE=%SOLUTION_DIR%\mobiflight.ico
SET BUILD_TOOLS_DIR=%~dp0
SET FW_SOURCE_DIR=%SOLUTION_DIR%\FirmwareSource
SET FW_BUILD_DIR=%SOLUTION_DIR%\FirmwareSource\tmp
SET TEMP_DIR=%FW_SOURCE_DIR%\tmp
SET ARDUINO_DIR=%FW_SOURCE_DIR%\arduino
SET VERSION=%2

IF "%1" == "/?"  goto USAGE
IF "%1" == ""    goto USAGE
IF "%2" == ""    goto USAGE

:RUN
echo -----------------------------------------------------------
echo Running command to create firmware files for version %VERSION%
echo -----------------------------------------------------------
echo Using %ARDUINO_DIR%\arduino-builder
mkdir %TEMP_DIR%
REM %ARDUINO_DIR%\arduino-builder -dump-prefs -logger=machine -hardware %ARDUINO_DIR%\hardware -hardware %FW_SOURCE_DIR%\packages -tools %ARDUINO_DIR%\tools-builder -tools %ARDUINO_DIR%\hardware\tools\avr -tools %FW_SOURCE_DIR%\packages -built-in-libraries %ARDUINO_DIR%\libraries -libraries %ARDUINO_DIR%\sketchbooks\libraries -fqbn=arduino:avr:mega:cpu=atmega2560 -vid-pid=0X2341_0X0042 -ide-version=10800 -build-path %TEMP_DIR% -warnings=default -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avrdude.path=%ARDUINO_DIR%\hardware\tools\avr -prefs=runtime.tools.avr-gcc.path=%ARDUINO_DIR%\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path=%ARDUINO_DIR%\hardware\tools\avr %FW_SOURCE_DIR%\mobiflight_mega\mobiflight_mega.ino
%ARDUINO_DIR%\arduino-builder -compile -logger=machine -hardware %ARDUINO_DIR%\hardware -hardware %FW_SOURCE_DIR%\packages -tools %ARDUINO_DIR%\tools-builder -tools %ARDUINO_DIR%\hardware\tools\avr -tools %FW_SOURCE_DIR%\packages -built-in-libraries %ARDUINO_DIR%\libraries -libraries %ARDUINO_DIR%\sketchbooks\libraries -fqbn=arduino:avr:mega:cpu=atmega2560 -vid-pid=0X2341_0X0042 -ide-version=10800 -build-path %TEMP_DIR% -warnings=default -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avrdude.path=%ARDUINO_DIR%\hardware\tools\avr -prefs=runtime.tools.avr-gcc.path=%ARDUINO_DIR%\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path=%ARDUINO_DIR%\hardware\tools\avr %FW_SOURCE_DIR%\mobiflight\mobiflight.ino
copy %TEMP_DIR%\mobiflight.ino.hex %RELEASE_DIR%\mobiflight_mega_%VERSION%.hex

REM %ARDUINO_DIR%\arduino-builder -dump-prefs -logger=machine -hardware %ARDUINO_DIR%\hardware -hardware %FW_SOURCE_DIR%\packages -tools %ARDUINO_DIR%\tools-builder -tools %ARDUINO_DIR%\hardware\tools\avr -tools %FW_SOURCE_DIR%\packages -built-in-libraries %ARDUINO_DIR%\libraries -libraries %ARDUINO_DIR%\sketchbooks\libraries -fqbn=SparkFun:avr:promicro:cpu=16MHzatmega32U4 -vid-pid=0X2341_0X0042 -ide-version=10800 -build-path %TEMP_DIR% -warnings=default -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avrdude.path=%ARDUINO_DIR%\hardware\tools\avr -prefs=runtime.tools.avr-gcc.path=%ARDUINO_DIR%\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path=%ARDUINO_DIR%\hardware\tools\avr %FW_SOURCE_DIR%\mobiflight_micro\mobiflight_micro.ino
%ARDUINO_DIR%\arduino-builder -compile -logger=machine -hardware %ARDUINO_DIR%\hardware -hardware %FW_SOURCE_DIR%\packages -tools %ARDUINO_DIR%\tools-builder -tools %ARDUINO_DIR%\hardware\tools\avr -tools %FW_SOURCE_DIR%\packages -built-in-libraries %ARDUINO_DIR%\libraries -libraries %ARDUINO_DIR%\sketchbooks\libraries -fqbn=SparkFun:avr:promicro:cpu=16MHzatmega32U4 -vid-pid=0X2341_0X0042 -ide-version=10800 -build-path %TEMP_DIR% -warnings=default -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avrdude.path=%ARDUINO_DIR%\hardware\tools\avr -prefs=runtime.tools.avr-gcc.path=%ARDUINO_DIR%\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path=%ARDUINO_DIR%\hardware\tools\avr %FW_SOURCE_DIR%\mobiflight\mobiflight.ino
copy %TEMP_DIR%\mobiflight.ino.hex %RELEASE_DIR%\mobiflight_micro_%VERSION%.hex

REM %ARDUINO_DIR%\arduino-builder -dump-prefs -logger=machine -hardware %ARDUINO_DIR%\hardware -hardware %FW_SOURCE_DIR%\packages -tools %ARDUINO_DIR%\tools-builder -tools %ARDUINO_DIR%\hardware\tools\avr -tools %FW_SOURCE_DIR%\packages -built-in-libraries %ARDUINO_DIR%\libraries -libraries %ARDUINO_DIR%\sketchbooks\libraries -fqbn=arduino:avr:uno -vid-pid=0X2341_0X0042 -ide-version=10800 -build-path %TEMP_DIR% -warnings=default -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avrdude.path=%ARDUINO_DIR%\hardware\tools\avr -prefs=runtime.tools.avr-gcc.path=%ARDUINO_DIR%\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path=%ARDUINO_DIR%\hardware\tools\avr %FW_SOURCE_DIR%\mobiflight_uno\mobiflight_uno.ino
%ARDUINO_DIR%\arduino-builder -compile -logger=machine -hardware %ARDUINO_DIR%\hardware -hardware %FW_SOURCE_DIR%\packages -tools %ARDUINO_DIR%\tools-builder -tools %ARDUINO_DIR%\hardware\tools\avr -tools %FW_SOURCE_DIR%\packages -built-in-libraries %ARDUINO_DIR%\libraries -libraries %ARDUINO_DIR%\sketchbooks\libraries -fqbn=arduino:avr:uno -vid-pid=0X2341_0X0042 -ide-version=10800 -build-path %TEMP_DIR% -warnings=default -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avrdude.path=%ARDUINO_DIR%\hardware\tools\avr -prefs=runtime.tools.avr-gcc.path=%ARDUINO_DIR%\hardware\tools\avr -prefs=runtime.tools.arduinoOTA.path=%ARDUINO_DIR%\hardware\tools\avr %FW_SOURCE_DIR%\mobiflight\mobiflight.ino
copy %TEMP_DIR%\mobiflight.ino.hex %RELEASE_DIR%\mobiflight_uno_%VERSION%.hex

echo -----------------------------------------------------------
echo DONE
echo -----------------------------------------------------------
goto END

:USAGE
echo --------------------------------------------------------------
echo Usage:
echo build-firmware.bat "absolute path to wokspace" "versionnumber"
echo e.g. build-firmware.bat D:\Mobiflight\ 3_0_1
echo --------------------------------------------------------------


:END