@echo off
echo Checking .NET Framework...

reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Release >nul 2>&1
if %errorlevel% neq 0 (
    echo .NET Framework 4.7.2 or higher required!
    echo Please install from: https://dotnet.microsoft.com/download/dotnet-framework
    pause
    exit /b 1
)

echo Starting application...
cd /d "%~dp0upcuaclient-vbnet\bin\Release"
start "" "upcuaclient-vbnet.exe"