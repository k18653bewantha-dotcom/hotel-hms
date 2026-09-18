@echo off
title Publish Kingswood Hotel PMS
dotnet restore
if errorlevel 1 pause & exit /b 1
dotnet publish -c Release -o publish
if errorlevel 1 pause & exit /b 1
echo.
echo Published files are in the publish folder.
pause
