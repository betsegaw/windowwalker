@echo off

set token=%1

pushd "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\VC\Auxiliary\Build"

call vcvarsall x86

popd

echo 'The current version on the server is '



FOR /F %%H IN ('Powershell.exe -executionpolicy remotesigned -File Deployment\GetVersion.ps1') DO set VERSION=%%H

cd "Window Walker"

powershell -Command "(New-Object System.Net.WebClient).DownloadFile(\"https://dist.nuget.org/win-x86-commandline/latest/nuget.exe\", \".\nuget.exe\")"

.\nuget.exe restore "Window Walker.sln"

msbuild /target:publish /p:Configuration=Release /property:ApplicationVersion=%VERSION% /p:Platform="Any CPU" /property:PublishUrl=".\..\Deployment\Deployment\"

cd ..

IF EXIST .\Deployment\Deployment ( 
	rmdir /s .\Deployment\Deployment /Q
)

mkdir .\Deployment\Deployment

xcopy ".\Window Walker\Window Walker\bin\Release\app.publish\*" ".\Deployment\Deployment\" /y /s

echo %VERSION%>.\Deployment\Deployment\version.txt

cd .\Deployment

REM for /f %%i in ('now') do set uniqueurl=%%i

powershell -Command "npm i -g now"

FOR /F %%G IN ('now --token %token% .') DO now --token %token% alias %%G download.windowwalker.com

cd ..

