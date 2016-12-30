#tool nuget:?package=Wyam&prerelease
#addin nuget:?package=Cake.Wyam&prerelease

var recipe = "Blog";
var theme = "CleanBlog";
var isLocal = BuildSystem.IsLocalBuild;

var target = Argument("target", isLocal ? "Default" : "CIBuild");

Task("Build")
    .Does(() => {
        Wyam(CreateSettings(false));
    });
    
Task("Preview")
    .Does(() => {
        Wyam(CreateSettings(true));        
    });
    
Task("Default")
    .IsDependentOn("Preview");    
    
Task("CIBuild")
    .IsDependentOn("Build");

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