using System.IO;

namespace Community.PowerToys.Run.Plugin.JetBrainsRecentProjects;

public record Project(
    string Name,
    string Path,
    string DisplayPath,
    Ide Ide
);

public record Ide(
    string ChannelId,
    string ToolId,
    string DisplayName,
    string InstallLocation,
    string LaunchCommand
)
{
    public string Executable => Path.Join(InstallLocation, LaunchCommand);
}