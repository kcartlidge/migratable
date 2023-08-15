clear
echo
echo PRODUCING CROSS-PLATFORM BUILDS
echo Should be run in the Migratable.CLI folder

echo
echo
echo ========
echo BUILDING
echo ========
echo
rm -rf ../builds/macos-x64
rm -rf ../builds/macos-arm64
rm -rf ../builds/linux-x64
rm -rf ../builds/win10-x64
dotnet build

echo
echo
echo ============================
echo PUBLISHING MACOS-X64 - Intel
echo ============================
echo
dotnet publish -o ../builds/macos-x64 -noconlog -r osx-x64 --self-contained -c Release /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true

echo
echo
echo ======================================
echo PUBLISHING MACOS-ARM64 - Apple Silicon
echo ======================================
echo
dotnet publish -o ../builds/macos-arm64 -noconlog -r osx-arm64 --self-contained -c Release /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true

echo
echo
echo ============================
echo PUBLISHING LINUX-X64 - Intel
echo ============================
echo
dotnet publish -o ../builds/linux-x64 -noconlog -r linux-x64 --self-contained -c Release /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true

echo
echo
echo ============================
echo PUBLISHING WIN10-X64 - Intel
echo ============================
echo
dotnet publish -o ../builds/win10-x64 -noconlog -r win10-x64 --self-contained -c Release /p:DebugType=None /p:DebugSymbols=false /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true

echo
echo
echo
echo DONE builds for:
echo
ls ../builds
echo
