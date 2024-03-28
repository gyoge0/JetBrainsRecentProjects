using System.IO;

namespace Community.PowerToys.Run.Plugin.JetBrainsRecentProjects;

public class Project(ProjectDto dto, Ide ide)
{
    public string Name { get; } = dto.Name;
    public string Path { get; } = dto.Path;
    public string DisplayPath { get; } = dto.DisplayPath;
    public Ide Ide { get; } = ide;
}

public class Ide(IdeDto dto)
{
    public string ChannelId { get; } = dto.ChannelId;
    public string DisplayName { get; } = dto.DisplayName;
    public string Executable => Path.Join(dto.InstallLocation, dto.LaunchCommand);
}