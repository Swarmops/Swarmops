if not exist C:\Lab\Swarmops\build\Deploy-Staging\BuildDropped.txt goto testSprint

cd \Lab\Swarmops\build\Deploy-Staging

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler.exe -u -v / -p frontend-raw -f -fixednames -errorstack frontend

if errorlevel 1 goto testSprint
C:\"Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools"\aspnet_merge.exe -o Swarmops.Site.dll frontend
rd /s /q frontend-raw
del \Lab\Swarmops\build\Deploy-Staging\BuildDropped.txt
robocopy \Lab\Swarmops\build\Deploy-Staging \\swarmops-dev\builddrop\swarmops\internal /MIR
echo foo > \\swarmops-dev\builddrop\swarmops\internal\BuildDropped.txt
cd ..
rd /s /q \Lab\Swarmops\build\Deploy-Staging



:testSprint

if not exist C:\Lab\Swarmops\sprint\Deploy-Staging\BuildDropped.txt goto endScript

cd \Lab\Swarmops\sprint\Deploy-Staging

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler.exe -u -v / -p frontend-raw -f -fixednames -errorstack frontend

if errorlevel 1 goto endScript
C:\"Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools"\aspnet_merge.exe -o Swarmops.Site.dll frontend
rd /s /q frontend-raw
del \Lab\Swarmops\sprint\Deploy-Staging\BuildDropped.txt
robocopy \Lab\Swarmops\sprint\Deploy-Staging \\swarmops-dev\builddrop\swarmops\sprint /MIR
echo foo > \\swarmops-dev\builddrop\swarmops\sprint\BuildDropped.txt
cd ..
rd /s /q \Lab\Swarmops\sprint\Deploy-Staging


:endScript
echo foo
