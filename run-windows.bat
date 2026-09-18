@echo off
title Kingswood Hotel PMS
dotnet restore
if errorlevel 1 pause & exit /b 1
dotnet run
pause
