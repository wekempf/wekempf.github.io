#tool nuget:?package=Wyam&prerelease
#addin nuget:?package=Cake.Wyam&prerelease

var recipe = "Blog";
var theme = "CleanBlog";
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
    
Task("Default")
    .IsDependentOn("Build");    
    
Task("Publish")
    .IsDependentOn("Build")
    .Does(() =>
    {
        if (GitClonePages() != 0) {
            throw new Exception("Unable to clone Pages.");
        }
        var dirs = GetDirectories("./pages/*")
            .Except(GetDirectories("./pages/.git"), DirectoryPathComparer.Default);
        DeleteDirectories(dirs, true);
        var files = GetFiles("./pages/*");
        DeleteFiles(files);
        CopyFiles("./output/**/*", "./pages", true);
        if (GitCommitPages() != 0) {
            throw new Exception("Unable to commit Pages.");
        }
        if (GitPushPages() != 0) {
            throw new Exception("Unable to publish Pages.");
        }
    });

RunTarget(target);

WyamSettings CreateSettings(bool preview)
{
    return new WyamSettings {
        Recipe = recipe,
        Theme = theme,
        Preview = preview,
        Watch = preview
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

public class DirectoryPathComparer : IEqualityComparer<DirectoryPath>
{
    public bool Equals(DirectoryPath x, DirectoryPath y)
    {
        return string.Equals(x.FullPath, y.FullPath, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(DirectoryPath x)
    {
        return x.FullPath.GetHashCode();
    }

    private static DirectoryPathComparer instance = new DirectoryPathComparer();
    public static DirectoryPathComparer Default
    {
        get { return instance; }
    }
}
