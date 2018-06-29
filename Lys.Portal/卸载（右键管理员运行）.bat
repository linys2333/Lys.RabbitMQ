net stop Lys.MQConsumer

cd /d %~dp0
dotnet.exe Lys.Portal.dll action:uninstall

pause