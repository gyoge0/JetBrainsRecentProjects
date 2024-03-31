using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.JetBrainsRecentProjects;

public partial class JetBrainsToolbox(string installLocation)
{
    public string InstallLocation { get; set; } = installLocation;

    private const string StateFilePath = "state.json";
    private const string IntelliJProjectsFilePath = "cache/intellij_projects.json";

    private string? GetFullPath(string localPath)
    {
        var fullPath = Path.Join(InstallLocation, localPath);

        if (Path.Exists(fullPath))
        {
            return fullPath;
        }

        Log.Error($"Path {fullPath} doesn't exist", typeof(JetBrainsToolbox));
        return null;
    }

    private T? DeserializePath<T>(string path, JsonSerializerSettings? settings = null) where T : class
    {
        T? deserialized = settings == null
            ? JsonConvert.DeserializeObject<T>(path)
            : JsonConvert.DeserializeObject<T>(path, settings);

        if (deserialized is not null)
        {
            return deserialized;
        }

        Log.Error($"Couldn't deserialize {path}", typeof(JetBrainsToolbox));
        return null;
    }

    private Tuple<StateDto, List<ProjectDto>>? ReadFiles()
    {
        string? stateFile = GetFullPath(StateFilePath);
        if (stateFile is null)
        {
            return null;
        }

        var stateText = File.ReadAllText(stateFile);
        var state = DeserializePath<StateDto>(stateText);
        if (state is null)
        {
            return null;
        }

        string? projectsFile = GetFullPath(IntelliJProjectsFilePath);
        if (projectsFile is null)
        {
            return null;
        }

        var projectsText = File.ReadAllText(projectsFile);
        var projects = DeserializePath<List<ProjectDto>>(
            projectsText,
            new JsonSerializerSettings
            {
                Error = (_, args) =>
                {
                    if (!ProjectLastModifiedJsonPath().IsMatch(args.ErrorContext.Path))
                    {
                        return;
                    }

                    Log.Info($"Skipping malformed datetime at {args.ErrorContext.Path}", typeof(JetBrainsToolbox));
                    args.ErrorContext.Handled = true;
                },
            }
        );
        if (projects is null)
        {
            return null;
        }

        return new(state, projects);
    }

    public List<Project> LoadProjects()
    {
        Tuple<StateDto, List<ProjectDto>>? tuple = ReadFiles();
        // ReSharper disable once IdentifierTypo
        if (tuple is not var (state, projectDtos))
        {
            return [];
        }

        var ides = state.Tools
            .Select(dto => new Ide(
                ChannelId: dto.ChannelId,
                ToolId: dto.ToolId,
                DisplayName: dto.DisplayName,
                InstallLocation: dto.InstallLocation,
                LaunchCommand: dto.LaunchCommand
            ))
            .Where(ide => Path.Exists(ide.Executable))
            .ToList();

        var idesByChannel = ides
            .ToDictionary(ide => ide.ChannelId);

        var idesByTool = ides
            .ToDictionary(ide => ide.ToolId);

        // TODO: simplify this?
        return projectDtos
            .Where(dto => Path.Exists(dto.Path))
            .ToDictionary(
                dto => dto,
                dto =>
                {
                    Ide? openWith = null;
                    if (idesByChannel.TryGetValue(dto.DefaultNewOpenItem, out var ide))
                    {
                        openWith = ide;
                    }

                    var preferredOrder = dto.NewOpenItems
                        .OrderByDescending(item => item.ProjectLastModified)
                        .ToList();

                    openWith ??= preferredOrder
                        .Select(item => item.ChannelId)
                        .Where(id => idesByChannel.ContainsKey(id))
                        .Select(id => idesByChannel[id])
                        .FirstOrDefault();

                    openWith ??= preferredOrder
                        .Select(item => item.ToolId)
                        .Where(id => idesByTool.ContainsKey(id))
                        .Select(id => idesByTool[id])
                        .FirstOrDefault();

                    return openWith;
                }
            )
            .Where(pair => pair.Value != null)
            .Select(pair => new Project(
                Name: pair.Key.Name,
                Path: pair.Key.Path,
                DisplayPath: pair.Key.DisplayPath,
                Ide: pair.Value!
            ))
            .ToList();
    }

    /// <summary>
    /// Matches the path of the projectLastModified attribute in cache/intellij_projects.json that can sometimes be malformed.
    /// </summary>
    [GeneratedRegex(@"\[\d+\]\.newOpenItems\[\d+\]\.projectLastModified")]
    private static partial Regex ProjectLastModifiedJsonPath();
}