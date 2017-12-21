#addin "nuget:?package=Cake.Services&version=0.2.9"
#addin "nuget:?package=Cake.Http&version=0.3.0"
#addin "nuget:?package=Cake.Powershell&version=0.3.5"
#addin "nuget:?package=Cake.Git&version=0.15.0"
#addin "Newtonsoft.Json"
#tool "nuget:?package=xunit.runner.console&version=2.2.0"
#tool "nuget:?package=OctopusTools&version=4.22.1"
#tool "nuget:?package=iisexpress.runner.service&version=0.1.5"
#tool "nuget:?package=SpecRun.Runner&version=1.6.0"
#tool "nuget:?package=NuGet.CommandLine&version=4.3.0"
/////////////////////////////////
// ARGUMENTS
/////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Debug");

/////////////////////////////////
//VARIABLES
/////////////////////////////////
var solutions = GetFiles("./**/*.sln");
var solutionPaths = solutions.Select(solution => solution.GetDirectory());

/////////////////////////////
// TASKS
/////////////////////////////

Task("Clean")
    .Description("Cleans all directories that are used during the build process.")
    .Does(() =>
{
    // Clean solution directories.
    foreach(var path in solutionPaths)
    {
        Information("Cleaning {0}", path);
        CleanDirectories(path + "/**/bin/" + configuration);
        CleanDirectories(path + "/**/obj/" + configuration);
    }
});

Task("Restore")
    .Description("Restores all packages that are used by all solutions.")
    .Does(() =>
{
    foreach(var solution in solutions)
    {
        Information("Restoring {0}...", solution);

        DotNetCoreRestore(solution.ToString());
    }
});

Task("Build")
    .Description("Builds all the different parts of the project.")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    // Build all solutions.
    foreach(var solution in solutions)
    {
        Information("Building {0}", solution);

        DotNetCoreBuild(solution.ToString(),  new DotNetCoreBuildSettings()
                                    {
                                        Configuration = configuration
                                    });
    }
});

Task("Test")
    .IsDependentOn("Test-Unit")
    .IsDependentOn("Test-Integration");

Task("Test-Unit")
    .Does(() =>
{
    var projects = GetFiles("./**/*Tests.Unit*.csproj");
        foreach(var project in projects)
        {
            DotNetCoreTest(
                project.FullPath,
                new DotNetCoreTestSettings()
                {
                    Configuration = "release",
                    NoBuild = true
                });
        }
});

Task("Test-Integration")
    .Does(() =>
{
    var projects = GetFiles("./**/*Tests.Integration*.csproj");
        foreach(var project in projects)
        {
            DotNetCoreTest(
                project.FullPath,
                new DotNetCoreTestSettings()
                {
                    Configuration = "release",
                    NoBuild = true
                });
        }
});

Task("Build+Test")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .Description("Runs Build and Test.")
    .IsDependentOn("Build+Test");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);