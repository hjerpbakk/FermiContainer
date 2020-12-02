#!/usr/bin/env bash
set -e
dotnet pack ./FermiContainer.Standard/FermiContainer.Standard.csproj --configuration Release --no-restore --no-build --output nupkgs