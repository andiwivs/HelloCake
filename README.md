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