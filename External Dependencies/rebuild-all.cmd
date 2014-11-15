@echo off
git submodule init
git submodule update

git submodule foreach git checkout master
git submodule foreach git pull

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild /p:Configuration=Release;TargetFrameworkVersion=v4.5 /t:Clean;Build NBitcoin\NBitcoin\NBitcoin.csproj

if exist bin erase /s /q bin > nul
if not exist bin mkdir bin
cd bin
xcopy /s ..\NBitcoin\NBitcoin\bin\Release\* .
cd ..
