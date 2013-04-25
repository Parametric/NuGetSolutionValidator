# NuGetSolutionValidator

### NugetSolutionValidator is a tool that is meant to do two things:
1. Ensure that any Nuget packages used in projects within a solution are the same version
2. Ensure that any NuSpec files built from projects within a solution contain their proper dependencies

Each of these are ran as unit tests using either NUnit or Microsoft's Unit Testing framework.  With these in place, any mismatches should be found during the execution of the tests manually, or during a CI build.

### Nuget
NugetSolutionValidator is available on the public Nuget feed at http://nuget.org/packages?q=nugetsolutionvalidator and can be installed by running the command:

#### NUnit
```
Install-Package NuGetSolutionValidator.NUnit
```

#### MsTest
```
Install-Package NuGetSolutionValidator.MsTest
```


### Library Structure
NugetSolutionValidator consists of the following projects:
* __NugetSolutionValidator:__ Domain Services library, this is where the knowledge of reading files and validating is kept
* __NugetSolutionValidator.NUnit:__ Contains a single NUnit TestFixture that runs validations in the NugetSolutionValidator library as tests
* __NugetSolutionValidator.MsTest:__ Contains a single Microsoft TestClass that runs validations in the NugetSolutionValidator library as tests
* __NugetSolutionValidator.Tests:__ Unit Test library that tests the Domain Services in NugetSolutionValidator
* __SampleProject:__ Sample project to see how the tests are installed and work

### Sample Usages
Given:
* Solution named MySolution
* NuSpec file name 'SomeGreatThing' that packages the projects: 'GreatProject1' and 'GreatProject2' libraries into a nuget package

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