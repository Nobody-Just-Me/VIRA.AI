@echo off
REM VIRA Build Script for Windows
REM This script automates the APK build process

setlocal enabledelayedexpansion

echo ========================================
echo    VIRA Build Script for Windows
echo ========================================
echo.

REM Check .NET SDK
echo [1/5] Checking prerequisites...
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] .NET SDK not found
    echo Please install .NET 8 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo [OK] .NET SDK found: %DOTNET_VERSION%

REM Check Android SDK
if "%ANDROID_HOME%"=="" (
    echo [WARNING] ANDROID_HOME not set
    echo Trying common locations...
    
    if exist "%LOCALAPPDATA%\Android\Sdk" (
        set ANDROID_HOME=%LOCALAPPDATA%\Android\Sdk
    ) else if exist "%USERPROFILE%\AppData\Local\Android\Sdk" (
        set ANDROID_HOME=%USERPROFILE%\AppData\Local\Android\Sdk
    ) else if exist "C:\Android\Sdk" (
        set ANDROID_HOME=C:\Android\Sdk
    ) else (
        echo [ERROR] Android SDK not found
        echo Please install Android SDK via Visual Studio or Android Studio
        exit /b 1
    )
)

echo [OK] Android SDK found: %ANDROID_HOME%
echo.

REM Clean previous builds
echo [2/5] Cleaning previous builds...
dotnet clean VIRA.Mobile\VIRA.Mobile.csproj -c Release
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Clean failed
    exit /b 1
)
echo.

REM Restore dependencies
echo [3/5] Restoring dependencies...
dotnet restore
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Restore failed
    exit /b 1
)
echo.

REM Build APK
echo [4/5] Building APK...
echo Configuration: Release
echo Target: net8.0-android
echo.

dotnet publish VIRA.Mobile\VIRA.Mobile.csproj ^
    -f net8.0-android ^
    -c Release ^
    -p:AndroidPackageFormat=apk ^
    -p:AndroidSdkDirectory=%ANDROID_HOME%

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Build failed
    exit /b 1
)

echo.
echo [5/5] Locating APK file...

REM Find APK file
set "APK_FILE="
for /r "VIRA.Mobile\bin\Release\net8.0-android" %%f in (*.apk) do (
    set "APK_FILE=%%f"
    goto :found
)

:found
if "%APK_FILE%"=="" (
    echo [ERROR] APK file not found
    exit /b 1
)

echo [OK] APK found: %APK_FILE%

REM Copy to root directory
copy "%APK_FILE%" "VIRA-Release.apk" >nul
if %ERRORLEVEL% EQU 0 (
    echo [OK] APK copied to: VIRA-Release.apk
)

REM Get file size
for %%A in ("VIRA-Release.apk") do set APK_SIZE=%%~zA
set /a APK_SIZE_MB=%APK_SIZE% / 1048576
echo APK Size: %APK_SIZE_MB% MB

echo.
echo ========================================
echo    BUILD SUCCESSFUL!
echo ========================================
echo.
echo APK Location: %CD%\VIRA-Release.apk
echo.
echo Installation Instructions:
echo 1. Transfer VIRA-Release.apk to your Android device
echo 2. Enable 'Install from Unknown Sources' in Settings
echo 3. Tap the APK file to install
echo 4. Open VIRA and enter your Gemini API Key in Settings
echo.
echo Or install via ADB:
echo    adb install -r VIRA-Release.apk
echo.
echo ========================================

endlocal
