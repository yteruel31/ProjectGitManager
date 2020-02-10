#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#tool "Squirrel.Windows" 
#addin "Cake.FileHelpers"
#addin Cake.Squirrel
#addin "Cake.Git"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./PGM.GUI/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./PGM.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./PGM.sln", settings => settings.SetConfiguration(configuration)
	  	.UseToolVersion(MSBuildToolVersion.VS2019)
		.SetVerbosity(Verbosity.Quiet)
		.SetNoLogo(true));
    }
    else
    {
      // Use XBuild
      XBuild("./PGM.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("IncrementVersionNumber")
	.Does(() => 
{
	
	var assemblypath = "./PGM.GUI/Properties/AssemblyInfo.cs";
	var assemblyInfo = ParseAssemblyInfo(assemblypath);

	var oldVersion = assemblyInfo.AssemblyVersion;
	var versions= oldVersion.Split('.');
	var newVersion = versions[0] + "." + versions[1] + "." + (int.Parse(versions[2])+1) + ".0";

	Information("OldVersion: {0}, new Version: {1}", oldVersion, newVersion);

    ReplaceRegexInFiles(assemblypath, 
                           "(?<=AssemblyVersion\\(\")(.+?)(?=\"\\))", 
                           newVersion);
    ReplaceRegexInFiles(assemblypath, 
                           "(?<=AssemblyFileVersion\\(\")(.+?)(?=\"\\))", 
                           newVersion);
});

Task("GitCommitAndPush")
	.Does(() =>
{
	var repoPath = System.IO.Path.GetFullPath(".");
	GitAddAll(repoPath);
	GitCommit(repoPath, "Yoann TERUEL", "yoann.teruel@gmail.eu", "Update version");
});


Task("Squirrel.Nuget")
	.Does(() =>
{
	
	var assemblypath = "./PGM.GUI/Properties/AssemblyInfo.cs";
	var assemblyInfo = ParseAssemblyInfo(assemblypath);

	var version = assemblyInfo.AssemblyVersion;
	version = version.Substring(0, version.Length - 2);

	Information($"version : {version}");

	var packSettings = new NuGetPackSettings 
	{
		OutputDirectory = "./Deployment/",
		Title = "ProjectGitManager",
		Summary = "Gestionnaire de Repository Gitlab",
		Description = "Gestionnaire de Repository Gitlab",
		Version = version,
		Id = "PGM.GUI",
		Authors = new []{ "Yoann TERUEL" },
        Owners = new[] {"Yoann TERUEL"},
		ProjectUrl = new Uri("https://gitlab.com/yteruel31/projectgitmanager"),
		Tags = new List<string> {"git", "gitlab"},
		Copyright = "Copyright Yoann TERUEL 2020",
		Properties = new Dictionary<string, string>
		{
			{ "Configuration", "Release" }
		}
	};

	Func<string,string,string> getRelativePath = (itemPath, basePath) => 
	{
		string fullPath = Directory(itemPath);
		string basePathDirectory = Directory(basePath);
		if(fullPath != basePathDirectory)
		{
			string ret = fullPath.Replace(basePathDirectory, "");
			return ret;
		}
		return "";
	};

	packSettings.Files = new List<NuSpecContent>();

	foreach (var item in GetFiles(MakeAbsolute(Directory("./PGM.GUI/bin/Release"))
		.FullPath.Replace("/", "\\") + @"\**\*"))
	{
		Information($"test : {item.FullPath}");
		if(!item.FullPath.ToLower().EndsWith(".pdb") || !item.FullPath.ToLower().EndsWith(".so"))
		{
			packSettings.Files.Add(new NuSpecContent
			{
				Source = item.FullPath,
				Target = "lib\\net45" + getRelativePath(item.FullPath, MakeAbsolute(Directory("./PGM.GUI/bin/Release")).FullPath)
			});
		}
	}

	NuGetPack(packSettings);
});

Task("Squirrel.Packaging")
	.Does(() =>
{
    var di = new DirectoryInfo("./Deployment/");
    var package = di.EnumerateFiles("*.nupkg").OrderBy(f => f.CreationTime).Last().FullName;

	var settings = new SquirrelSettings();
	settings.ReleaseDirectory = @"./Release";
	Information($"Chemin : {settings.ReleaseDirectory}");

	Squirrel(package, settings);
});

Task("Squirrel.FileCopy")
	.Does(() =>
{
	var sourcePath = @"D:\tempSetup\*";
	var targetDirectory = @"\\serveur-nas1\USERS\Epinat\MantaGitManager";
	var files = GetFiles(sourcePath);
	CopyFiles(files , targetDirectory, true);
});

Task("Squirrel.Total")
    .IsDependentOn("IncrementVersionNumber")
    .IsDependentOn("Build")
    .IsDependentOn("Squirrel.Nuget")
    .IsDependentOn("Squirrel.Packaging");

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
	.IsDependentOn("Squirrel.Total");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
