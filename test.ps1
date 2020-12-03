Remove-Item ./CoverageResults -Force -Recurse -ErrorAction SilentlyContinue
dotnet test ./FermiContainer.Tests/FermiContainer.Tests.csproj --configuration Release /p:CollectCoverage=true /p:CoverletOutput="../CoverageResults/" /p:CoverletOutputFormat=opencover /p:Threshold=100
