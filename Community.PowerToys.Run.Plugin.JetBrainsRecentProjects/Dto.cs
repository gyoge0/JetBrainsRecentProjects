using Newtonsoft.Json;
// ReSharper disable ClassNeverInstantiated.Global

namespace Community.PowerToys.Run.Plugin.JetBrainsRecentProjects;

public record StateDto(
    List<IdeDto> Tools
);

public record IdeDto(
    string ChannelId,
    string DisplayName,
    string InstallLocation,
    string LaunchCommand
);

public record ProjectDto(
    string Name,
    string Path,
    string DisplayPath,
    [JsonProperty("defaultNewOpenItem")] string DefaultIde
);