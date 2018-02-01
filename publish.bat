@echo off

pushd "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\VC\Auxiliary\Build"

call vcvarsall x86

popd

echo 'The current version on the server is '

Powershell.exe -executionpolicy remotesigned -File Deployment\GetVersion.ps1

set /p VERSION="Enter new version number you would like to publish:"

cd "Window Walker"

msbuild /target:publish /p:Configuration=Release /property:ApplicationVersion=%VERSION% /p:Platform="Any CPU" /property:PublishUrl=".\..\Deployment\Deployment\"

cd ..

IF EXIST .\Deployment\Deployment ( 
	rmdir /s .\Deployment\Deployment /Q
)

mkdir .\Deployment\Deployment

xcopy ".\Window Walker\Window Walker\bin\Release\app.publish\*" ".\Deployment\Deployment\" /y /s

echo %VERSION%>.\Deployment\Deployment\version.txt

cd .\Deployment

now .

