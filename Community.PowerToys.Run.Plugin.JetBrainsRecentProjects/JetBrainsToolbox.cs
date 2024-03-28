using System.IO;
using Newtonsoft.Json;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.JetBrainsRecentProjects;

public class JetBrainsToolbox(string installLocation)
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

    private T? DeserializePath<T>(string path) where T : class
    {
        T? deserialized = JsonConvert.DeserializeObject<T>(path);
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
        var projects = DeserializePath<List<ProjectDto>>(projectsText);
        if (projects is null)
        {
            return null;
        }

        return new(state, projects);
    }

    public List<Project> LoadProjects()
    {
        Tuple<StateDto, List<ProjectDto>>? tuple = ReadFiles();
        if (tuple is null)
        {
            return [];
        }

        var ides = tuple.Item1.Tools
            .Select(dto => new Ide(dto))
            .ToDictionary(ide => ide.ChannelId);

        var projects = tuple.Item2
            .Select(dto => new Project(dto, ides[dto.DefaultIde]))
            .ToList();

        return projects;
    }
}