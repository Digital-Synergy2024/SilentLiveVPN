@echo off
:: Use %USERPROFILE% to get the current user's directory
set "sourceDir=%USERPROFILE%\Desktop\MyVpn\source\SilentLiveVPN\SilentLiveVPN\bin\Release\"
set "outputFile=%USERPROFILE%\Desktop\MyVpn\source\SilentLiveVPN\SetupSilentVPN\GeneratedFileList.wxs"
set "outputDir=%USERPROFILE%\Desktop\MyVpn\source\SilentLiveVPN\SetupSilentVPN\"
cd %WIX%bin
heat dir %sourceDir% -cg ComponentGroup -dr INSTALLFOLDER -gg -sreg -srd -ag -sfrag -suid -t %outputDir%Remove.xslt -o %outputFile%
cd %outputDir%
:: Convert wix file V3 to V4 
wix convert GeneratedFileList.wxs
pause
