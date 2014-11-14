if not exist C:\Lab\Swarmops\build\Deploy-Staging\BuildDropped.txt goto testSprint
cd \Lab\Swarmops\build\Deploy-Staging
C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_compiler.exe -u -v / -p frontend-raw -f -fixednames -errorstack frontend
C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_merge.exe -o Swarmops.Site.dll frontend

:testSprint
echo ending

