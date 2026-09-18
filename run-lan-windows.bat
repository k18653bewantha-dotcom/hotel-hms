@echo off
title Kingswood Hotel PMS - LAN
dotnet restore
if errorlevel 1 pause & exit /b 1
dotnet run --urls "http://0.0.0.0:5050"
pause
