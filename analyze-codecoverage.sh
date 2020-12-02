#!/usr/bin/env bash
set -e
./build.sh
./test.sh
dotnet ~/.nuget/packages/reportgenerator/4.6.7/tools/netcoreapp3.0/ReportGenerator.dll -reports:./CoverageResults/coverage.opencover.xml -targetdir:./Tests/CoverageResults
open ./Tests/CoverageResults/index.htm