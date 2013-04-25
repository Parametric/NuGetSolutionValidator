# NuGetSolutionValidator

### NugetSolutionValidator is a tool that is meant to do two things:
1. Ensure that any Nuget packages used in projects within a solution are the same version
2. Ensure that any NuSpec files built from projects within a solution contain their proper dependencies

### Nuget
NugetSolutionValidator is available on the public Nuget feed at http://nuget.org/packages?q=nugetsolutionvalidator and can be installed by running the command:

```
Install-Package NuGetSolutionValidator.NUnit
```

### Project Structure


### Sample Usages
Given a solution named MySolution and a NuSpec file name 'SomeGreatThing' that packages the 'GreatProject1' and 'GreatProject2' libraries into a nuget package
In the NuGetValidationTests test fixture:

#### Verify that all packages are the same version, excluding test projects
```c#
	var solutionBuilder = new SolutionBuilder();
	var request = new BuildSolutionRequest()
		.WithSolutionName("MySolutionName")
		.WithProjects(p => !p.Name.EndsWith("Test"))
		;
	_solution = solutionBuilder.Build(request);
```

#### Verify that a nuspec package has all its dependencies configured
```c#
	var solutionBuilder = new SolutionBuilder();
	var request = new BuildSolutionRequest()
		.WithSolutionName("MySolution")
		.WithNuSpec("SomeGreatThing")
		.WithNuSpecProjectSet("SomeGreatThing", new[] { "GreatProject1","GreatProject2" })
		;

	_solution = solutionBuilder.Build(request);
```