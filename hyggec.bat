@echo off
for /f "tokens=1 delims=." %%a in ('dotnet --version') do set DOTNET_VERSION=%%a.%%b
set FRAMEWORK_VERSION=net%DOTNET_VERSION%
set ARGS=-v q --framework %FRAMEWORK_VERSION% -c Debug

dotnet build %ARGS% > nul 2>&1
dotnet run --no-build %ARGS% -- %*
