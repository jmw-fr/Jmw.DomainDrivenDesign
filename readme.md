# Domain Driven Design .Net Library

This library helps DDD implementation by defining interfaces and abstract classes for Repositories.

# Code coverage

Code coverage uses coverlet (https://github.com/tonerdo/coverlet) for code coverage and reportgenerator (https://github.com/danielpalme/ReportGenerator)
to generate the report.

To generate code coverage :
``` powershell
dotnet test Jmw.DomainDrivenDesign.sln --configuration Release --logger trx /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Exclude="[xunit*]*%2c[*]GitVersionInformation"
```

To generate report :
``` powershell
dotnet $env:userprofile\.nuget\packages\reportgenerator\4.0.8\tools\netcoreapp2.0\ReportGenerator.dll "-reports:$pwd\**\coverage.cobertura.xml" "-targetdir:$pwd/coveragereport" "-reporttypes:Cobertura;HtmlInline_AzurePipelines"
```

