if not exist C:\Lab\Swarmops\build\Deploy-Staging\BuildDropped.txt goto testSprint
cd \Lab\Swarmops\build\Deploy-Staging
aspnet_compiler.exe -u -v / -p frontend-raw -f -fixednames -errorstack frontend
aspnet_merge.exe -o Swarmops.Site.dll frontend

:testSprint
echo ending

