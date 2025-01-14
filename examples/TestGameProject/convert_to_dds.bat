@echo off
nvdxt.exe -file %1 -dxt5 -outsamedir
set filename=%~dpn1
set extension=%~x1
SET normalname=%filename%_n%extension%
if exist %normalname% nvdxt.exe -file %normalname% -dxt5 -norm -outsamedir
pause