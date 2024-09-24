@echo off
cd "C:\Program Files (x86)\WiX Toolset v3.14\bin"

:: Use %USERPROFILE% to get the current user's directory
set "sourceDir=%USERPROFILE%\Desktop\MyVpn\source\SilentLiveVPN\SetupSilentVPN\"
set "outputFile=%USERPROFILE%\Desktop\MyVpn\source\SilentLiveVPN\SetupSilentVPN\GeneratedFileList.wxs"

heat.exe dir "%sourceDir%" -cg ComponentGroup -dr INSTALLFOLDER -sreg -srd -ag -sfrag -suid -o "%outputFile%"

pause
