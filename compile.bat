@echo off
REM Script de compilation du OBS Stream Music Viewer

echo Compilation de l'interface graphique (OBS-StreamMusicViewer.exe)...
echo.

REM Vérifier si dotnet est installé
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERREUR: .NET SDK n'est pas installe.
    echo.
    echo Telechargez et installez .NET SDK depuis:
    echo https://dotnet.microsoft.com/download
    echo.
    pause
    exit /b 1
)

REM Compiler le projet
dotnet publish OBS-StreamMusicViewer.csproj -c Release -o .

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ============================================
    echo Compilation reussie!
    echo L'executable OBS-StreamMusicViewer.exe est pret.
    echo Vous pouvez le lancer directement.
    echo ============================================
    echo.
) else (
    echo.
    echo ============================================
    echo ERREUR lors de la compilation
    echo ============================================
    echo.
)

pause
