@echo off
title AuthVaultix Native AOT Compiler
color 0b
cls
echo =======================================================
echo     AuthVaultix Native AOT Standalone Compiler
echo =======================================================
echo.

:: Check .NET SDK
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    color 0c
    echo [!] ERROR: .NET SDK is not installed or not in PATH!
    echo Please download and install .NET 8.0 SDK from:
    echo https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
    pause
    exit /b 1
)

echo [*] Cleaning previous builds...
dotnet clean -c Release >nul 2>&1

echo [*] Restoring NuGet dependencies...
dotnet restore
if %errorlevel% neq 0 (
    color 0c
    echo [!] ERROR: Restore failed!
    pause
    exit /b %errorlevel%
)

echo.
echo =======================================================
echo  Compiling Native Code (This may take a few minutes)...
echo  Please make sure C++ Build Tools (MSVC) are installed.
echo =======================================================
echo.

dotnet publish -r win-x64 -c Release

if %errorlevel% neq 0 (
    color 0c
    echo.
    echo [!] ERROR: Native AOT Compilation failed!
    echo Ensure Visual Studio C++ Build Tools workload is installed.
    echo.
    pause
    exit /b %errorlevel%
)

color 0a
echo.
echo =======================================================
echo  [+] SUCCESS: Standalone Native Executable Generated!
echo =======================================================
echo.
echo Output path:
echo bin\Release\net8.0\win-x64\publish\AuthVaultixNativeAotExample.exe
echo.

set /p run="Do you want to run the compiled native app now? (y/n): "
if /i "%run%"=="y" (
    cls
    color 07
    bin\Release\net8.0\win-x64\publish\AuthVaultixNativeAotExample.exe
)

pause
