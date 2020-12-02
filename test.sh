#!/usr/bin/env bash
set -e
rm -f -r ./CoverageResults
dotnet test ./FermiContainer.Tests/FermiContainer.Tests.csproj --configuration Release /p:CollectCoverage=true /p:CoverletOutput="../CoverageResults/" /p:CoverletOutputFormat=opencover /p:Threshold=100
