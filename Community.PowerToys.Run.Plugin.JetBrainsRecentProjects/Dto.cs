// ReSharper disable ClassNeverInstantiated.Global

namespace Community.PowerToys.Run.Plugin.JetBrainsRecentProjects;

public record StateDto(
    List<IdeFromStateDto> Tools
);

public record IdeFromStateDto(
    string ChannelId,
    string ToolId,
    string DisplayName,
    string InstallLocation,
    string LaunchCommand
);

public record IdeFromProjectsDto(
    string ChannelId,
    string ToolId,
    DateTime ProjectLastModified
);

public record ProjectDto(
    string Name,
    string Path,
    string DisplayPath,
    string DefaultNewOpenItem,
    List<IdeFromProjectsDto> NewOpenItems
);