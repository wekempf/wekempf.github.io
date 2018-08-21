#tool nuget:?package=Wyam
#addin nuget:?package=Cake.Wyam

var isLocal = BuildSystem.IsLocalBuild;
var gitPagesRepo = isLocal
    ? "git@github.com:wekempf/wekempf.github.io.git"
    : "https://wekempf:" + EnvironmentVariable("GitHubPersonalAccessToken") + "@github.com/wekempf/wekempf.github.io.git";
var gitPagesBranch = "master";

var target = Argument("target", isLocal ? "Default" : "CIBuild");

Task("Clean")
    .Does(() => {
        if (DirectoryExists("./output")) DeleteDirectory("./output", true);
    });

Task("Build")
    .IsDependentOn("Clean")
    .Does(() => {
        Wyam(CreateSettings(false));
    });
    
Task("Preview")
    .Does(() => {
        Wyam(CreateSettings(true));        
    });
    
Task("Publish")
    .IsDependentOn("Build")
    .Does(() =>
    {
        if (GitClonePages() != 0) {
            throw new Exception("Unable to clone Pages.");
        }
        var dirs = GetDirectories("./pages/*") - GetDirectories("./pages/.git");
        DeleteDirectories(dirs, true);
        var files = GetFiles("./pages/*") - GetFiles("./pages/CNAME");
        foreach (var f in files) Information(f.FullPath);
        DeleteFiles(files);
        CopyFiles("./output/**/*", "./pages", true);
        if (GitCommitPages() != 0) {
            throw new Exception("Unable to commit Pages.");
        }
        if (GitPushPages() != 0) {
            throw new Exception("Unable to publish Pages.");
        }
    });

Task("Default")
    .IsDependentOn("Build");    
    
RunTarget(target);

WyamSettings CreateSettings(bool preview)
{
    return new WyamSettings {
        Preview = preview,
        Watch = preview,
        //Verbose = true
    };
}

int GitCommand(string command, string workingDirectory = null) {
    Information("git " + command);
    var settings = new ProcessSettings { Arguments = command };
    if (workingDirectory != null) settings.WorkingDirectory = workingDirectory;
    return StartProcess("git", settings);
}

int GitClonePages() {
    if (DirectoryExists("./pages")) {
        return 0;
    }
    return GitCommand("clone " + gitPagesRepo + " -b " + gitPagesBranch + " pages");
}

int GitCommitPages() {
    var result = GitCommand("add .", "./pages");
    if (result != 0) {
        return result;
    }
    result = GitCommand("commit -m \"Publishing pages " + DateTime.Now + "\"", "./pages");
    return result;
}

int GitPushPages() {
    return GitCommand("push", "./pages");
}
