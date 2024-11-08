@echo off
@cls
@echo.
@echo PRODUCING CROSS-PLATFORM BUILDS
@echo Should be run in the Migratable.CLI folder

@echo.
@echo.
@echo ========
@echo BUILDING
@echo ========
@echo.
@rd /S /Q builds 2>NUL
@dotnet build

@echo.
@echo.
@echo ============================
@echo PUBLISHING MACOS-X64 - Intel
@echo ============================
@echo.
@dotnet publish -o ..\builds\macos-x64 -noconlog -r osx-x64 --self-contained -c Release /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true

@echo.
@echo.
@echo ======================================
@echo PUBLISHING MACOS-ARM64 - Apple Silicon
@echo ======================================
@echo.
@dotnet publish -o ..\builds\macos-arm64 -noconlog -r osx-arm64 --self-contained -c Release /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true

@echo.
@echo.
@echo ============================
@echo PUBLISHING LINUX-X64 - Intel
@echo ============================
@echo.
@dotnet publish -o ..\builds\linux-x64 -noconlog -r linux-x64 --self-contained -c Release /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true

@echo.
@echo.
@echo ==========================
@echo PUBLISHING WIN-X64 - Intel
@echo ==========================
@echo.
@dotnet publish -o ..\builds\win-x64 -noconlog -r win-x64 --self-contained -c Release /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true

@echo.
@echo.
@echo.
@echo DONE builds for:
@echo.
@dir /B ..\builds
@echo.
