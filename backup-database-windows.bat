@echo off
if not exist backups mkdir backups
set stamp=%date:~-4,4%%date:~-10,2%%date:~-7,2%_%time:~0,2%%time:~3,2%%time:~6,2%
set stamp=%stamp: =0%
copy /Y hotel.db backups\hotel_%stamp%.db
echo Database backup created in the backups folder.
pause
