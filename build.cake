///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target          = Argument<string>("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var isLocalBuild        = !AppVeyor.IsRunningOnAppVeyor;
var isPullRequest       = AppVeyor.Environment.PullRequest.IsPullRequest;
var isTag               = AppVeyor.Environment.Repository.Tag.IsTag;
var solutions           = GetFiles("./**/*.sln");
var solutionPaths       = solutions.Select(solution => solution.GetDirectory());
var version             = "0.0.1";
var binDir              = "./src/Cake.Prompt/bin/" + configuration;
var nugetRoot           = "./nuget/";
var semVersion          = isLocalBuild
                                ? version
                                : string.Concat(version, "-build-", AppVeyor.Environment.Build.Number);
var assemblyInfo        = new AssemblyInfoSettings {
                                Title                   = "Cake.Prompt",
                                Description             = "Cake Prompt AddIn",
                                Product                 = "Cake.Prompt",
                                Version                 = version,
                                FileVersion             = version,
                                InformationalVersion    = semVersion,
                                Copyright               = string.Format("Copyright © Yves Schmid {0}", DateTime.Now.Year),
                                CLSCompliant            = true
                            };
var nuGetPackSettings   = new NuGetPackSettings {
                                Id                      = assemblyInfo.Product,
                                Version                 = assemblyInfo.InformationalVersion,
                                Title                   = assemblyInfo.Title,
                                Authors                 = new[] {"Yves Schmid"},
                                Owners                  = new[] {"Yves Schmid"},
                                Description             = assemblyInfo.Description,
                                Summary                 = "Cake AddIn that extends Cake with user interactive prompt features",
                                ProjectUrl              = new Uri("https://github.com/yvschmid/Cake.Prompt/"),
                                LicenseUrl              = new Uri("https://github.com/yvschmid/Cake.Prompt/"),
                                Copyright               = assemblyInfo.Copyright,
                                Tags                    = new [] {"Cake", "Script", "Build", "Prompt"},
                                RequireLicenseAcceptance= false,
                                Symbols                 = false,
                                NoPackageAnalysis       = true,
                                Files                   = new [] {
                                                                    new NuSpecContent {Source = "Cake.Prompt.dll"},
                                                                    new NuSpecContent {Source = "Cake.Prompt.pdb"},
                                                                    new NuSpecContent {Source = "Cake.Prompt.xml"}
                                                                 },
                                BasePath                = binDir,
                                OutputDirectory         = nugetRoot
                            };

if (!isLocalBuild)
{
    AppVeyor.UpdateBuildVersion(semVersion);
}

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(() =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");

    var buildStartMessage = string.Format(
                            "Building version {0} of {1} ({2}).",
                            version,
                            assemblyInfo.Product,
                            semVersion
                            );

    Information(buildStartMessage);
});

Teardown(() =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
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
    .Does(() =>
{
    // Restore all NuGet packages.
    foreach(var solution in solutions)
    {
        Information("Restoring {0}...", solution);
        NuGetRestore(solution);
    }
});

Task("SolutionInfo")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var file = "./src/SolutionInfo.cs";
    CreateAssemblyInfo(file, assemblyInfo);
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("SolutionInfo")
    .Does(() =>
{
    // Build all solutions.
    foreach(var solution in solutions)
    {
        Information("Building {0}", solution);
        MSBuild(solution, settings =>
            settings.SetPlatformTarget(PlatformTarget.MSIL)
                .WithProperty("TreatWarningsAsErrors","true")
                .WithTarget("Build")
                .SetConfiguration(configuration));
    }
});


Task("Create-NuGet-Package")
    .IsDependentOn("Build")
    .Does(() =>
{
    if (!DirectoryExists(nugetRoot))
    {
        CreateDirectory(nugetRoot);
    }
    NuGetPack(nuGetPackSettings);
});

Task("Publish-To-MyGet")
	.IsDependentOn("Create-NuGet-Package")
	.WithCriteria(() => !isLocalBuild)
	.WithCriteria(() => !isPullRequest)
	.Does(() =>
{
	var apiKey = EnvironmentVariable("MYGET_API_KEY");
	var source = EnvironmentVariable("MYGET_SOURCE");
	var package = nugetRoot + "Cake.Prompt." + semVersion + ".nupkg";

	NuGetPush(package, new NuGetPushSettings {
		Source = source,
		ApiKey = apiKey
	});
});

Task("Publish-To-NuGet")
	.IsDependentOn("Create-NuGet-Package")
	.WithCriteria(() => !isLocalBuild)
	.WithCriteria(() => !isPullRequest)
	.WithCriteria(() => isTag)
	.Does(() =>
{
	var apiKey = EnvironmentVariable("NUGET_API_KEY");
	var source = EnvironmentVariable("NUGET_SOURCE");
	var package = nugetRoot + "/Cake.Prompt." + semVersion + ".nupkg";

	NuGetPush(package, new NuGetPushSettings {
		Source = source,
		ApiKey = apiKey
	});
});


Task("Default")
	.IsDependentOn("Create-NuGet-Package");
	
Task("AppVeyor")
	.IsDependentOn("Publish-To-MyGet")
	.IsDependentOn("Publish-To-NuGet");


///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
