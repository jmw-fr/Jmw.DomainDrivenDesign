# C# Class Library Template

This template creates a new project with the following elements :

- src / test directories,
- a class library in netstandard 2.0,
- xUnit test project,
- StyleCop nuget package,
- GitVersionTask package.

## StyleCop

StyleCop is used for source code quality. StyleCop is configured with the stylecop.json and stylecop.ruleset files. Both are incluced in the `Jmw.ddd` and `Jmw.dddUnitTest` projects.

## GitVersionTask

GitversionTask is used to automate the versioning of the class library.

You must create the project with Git and an initial commit otherwise you might experience an error when compiling.
