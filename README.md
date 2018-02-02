To run, use the following command:  
>.\build.ps1 -Target greet  
  
  
GETTING STARTED  
You begin with an empty build.cake script file, and from within Visual Studio (having installed the Cake extension) go to:  
Build > Cake Build > Install PowerShell bootstrapper  
  
You will now find a build.ps1 script added to the solution alongside the build.cake script.  
  
  
ALIASES  
```c#
Task("SomeAlias")               // this is how we define an alias  
    .Does(() => { ... });
```  
  
  
DEPENDENCIES  
```c#
Task("Step3")  
    .IsDependentOn("Step1")     // the order we specify dependencies determines the order they execute  
    .IsDependentOn("Step2")  
    .Does(() => { ...});
```  
  
  
CRITERIA  
```c#
Task("SomeTask")  
    .WithCriteria(false)        // use a boolean expression to conditionally run a task  
    .Does(() => { ...});
```  
  
  
ERROR HANDLING  
```c#
Task("SomeFlakeyTask")  
    .ContinueOnError()          // On Error Resume Next says "hi"  
    .Does(() => { ...});  
    
Task("SomeFlakeyTask")  
    .Does(() => { ...})  
    .ReportError(exception =>  
    {  
        // custom error logging goes here  
    });  
      
Task("SomeFlakeyTask")  
    .Does(() => { ...})  
    .OnError(exception =>  
    {  
        // handle errors here  
    });
```  
  
  
ABORTING  
```c#
Task("SomeTask")  
    .WithCriteria(false)        // use a boolean expression to conditionally run a task  
    .Does(() =>   
    {  
        if (someCondition)  
        {  
            throw new Exception("something bad happened")  
        }  
    });
```  
  
  
SETUP & TEARDOWN  
```c#
Setup((CakeContext context) =>  
{  
    // runs before the first task  
});  
  
Teardown((CakeContext context) =>  
{  
    // runs after the last task  
});  
  
TaskSetup((CakeContext context, CakeTask task) =>  
{  
    // runs before every task begins  
});  
  
TaskTeardown((CakeContext context, CakeTask task) =>  
{  
    // runs after every task ends  
});
```  
  
  
CONTEXT  
```c#
Task("SomeTask")  
    .Does(() =>  
    {  
        // eg logging  
        var workingDirectory = Context.Environment.WorkingDirectory;  
  
        Context.Log.Write(  
            Verbosity.Diagnostic,  
            LogLevel.Verbose,  
            "Working directory: {0}",  
            workingDirectory  
        );  
  
        // ...using alias shorthand:  
        Verbose("Working directory: {0}", workingDirectory);  
  
  
        // eg environment variables  
        var someVariable = Context.Environment.GetEnvironmentVariables()["some_var_key"];  
          
        // ...using alias shorthand:  
        var someVariable = EnvironmentVariable("some_var_key");  
    });
```  
  
LOADING TOOLS
We can load in tools - such as the NUnit test runner application - from NuGet by including a `#tool` pre-processor directive:  
  
```c#
#tool "nuget:?package=NUnit.Runners&version=2.6.4"
```  
  
CUSTOM TOOL PATH  
By default, NuGet tool resolution occurs in the local Tools folder. However, we can override this with an alias setting:
  
```c#
Alias(new AliasSettings
{
    ToolPath = @"C:\Path\To\Tool.exe"
});
```
  
  
CUSTOM TOOLS
In this scenario, we want to utilise a build tool that cannot be referenced by NuGet. 
We begin by referencing the tool location during setup:  
  
```c#
Setup(context =>  
{
    context.Tools.RegisterFile(@"C:\some\program.exe");
});
```  
  
Then, in our build task we create can run the tool as required:
  
```c#
Task("SomeTask")
    .Does(() =>
    {
        var path = Context.Tools.Resolve("program.exe");
        StartProcess(path);
    });
```  
  
SCALING UP - CUTTING THE CAKE  
As a cake script grows, it might make sense to break the script into smaller files. 
In addition, we may want to create utility code that can be shared between different cake scripts.  
  
```c#
#load relative/path/to/other/script.cake

// you can also read in scripts from a NuGet package source, where all .cake scripts will be loaded
// note: this is not supported in .net core (yet?)
#load nuget:?package=Name
```  
  
REPORTING TEST COVERAGE
Note: this is not yet possible to use with .net core.  
  
```c#
#tool nuget:?package=xunit.runner.console&version=2.2.0
#tool nuget:?package=OpenCover&version=4.6.519
#tool nuget.?package=ReportGenerator&version=2.5.8

var codeCoverageReportPath = Argument<FilePath>("CodeCoverageReportPath", "coverage.zip")

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        OpenCover(
            tool => tool.XUnit2("**/bin/" + configuration + "/*.Tests.dll"),
            new Xunit2Settings
            {
                ShadowCopy = false              // prevent XUnit from working on "shadow" copy of assemblies
            },
            @"c:\path\to\coverage\output\results",
            new OpenCoverSettings()
                    .WithFilter("+[Site.*]*")   // include projects beginning "Site."
                    .WithFilter("-[*Tests*]")   // exclude projects that are tests
        );
    });

Task("Report-Coverage")
    .IsDependentOn("Test")
    .Does(() =>
    {
        ReportGenerator(
            @"c:\path\to\coverage\output\results",
            @"c:\path\to\report\output\results",
            new ReportGeneratorSettings
            {
                ReportTypes = new[] { ReportGeneratorReportType.Html }
            }
        );

        Zip(
            @"c:\path\to\report\output\results",
            MakeAbsolute(codeCoverageReportPath)
        );
    });
```  
