var target = Argument<string>("Target");

Task("SayHello")
    .Does(() =>
    {
        Information("Hello");
    });

Task("SayWabbit")
    .Does(() =>
    {
        Information("Wabbit");
    });

Task("Greet")
    .IsDependentOn("SayHello")
    .IsDependentOn("SayWabbit")
    .Does(() =>
    {
        Information("All done");
    });

RunTarget(target);