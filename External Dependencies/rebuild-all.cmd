@echo off

:: Make sure that the submodules are properly initialized

git submodule init
git submodule update

:: Check out latest version of each

git submodule foreach git checkout master
git submodule foreach git pull

:: Build all

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild /p:Configuration=Release;TargetFrameworkVersion=v4.5 /t:Clean;Build NBitcoin\NBitcoin\NBitcoin.csproj

:: Copy builds to output

if exist bin erase /s /q bin > nul
if not exist bin mkdir bin
cd bin
xcopy /s ..\NBitcoin\NBitcoin\bin\Release\* .
cd ..

:: Revert submodules as to not dirty the Git Status

git submodule update
