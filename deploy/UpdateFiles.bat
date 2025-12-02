@echo off
copy /Y ..\Doc\ReleaseNote.txt InnoSetup\Files
copy /Y ..\Doc\ChangeLog.txt InnoSetup\Files\x86
copy /Y ..\UpdateFiles\*.exe InnoSetup\Files\x86
copy /Y ..\UpdateFiles\*.dll InnoSetup\Files\x86
