using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using Microsoft.PowerToys.Settings.UI.Library;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.JetBrainsRecentProjects;

using Wox.Plugin;

// ReSharper disable once UnusedType.Global
public class Main : IPlugin, IDelayedExecutionPlugin, ISettingProvider
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
    public static string PluginID => "E3318C75-337A-4A11-918A-90BB4F909502";

    public string Name => "JetBrains Recent Projects";

    public string Description => "Open recent projects from JetBrains IDEs";


    private static string DefaultInstallLocation { get; } =
        Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "JetBrains/Toolbox"
        );

    private JetBrainsToolbox Toolbox { get; } = new(DefaultInstallLocation);
    private const string ToolboxInstallLocationKey = "ToolboxInstallLocation";


    public IEnumerable<PluginAdditionalOption> AdditionalOptions =>
    [
        new()
        {
            Key = ToolboxInstallLocationKey,
            DisplayLabel = "JetBrains Toolbox data directory",
            PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
            TextValue = DefaultInstallLocation,
        }
    ];

    public void UpdateSettings(PowerLauncherPluginSettings settings)
    {
        PluginAdditionalOption? location = settings.AdditionalOptions
            ?.First(x => x.Key == ToolboxInstallLocationKey);

        if (location is null)
        {
            return;
        }

        Toolbox.InstallLocation = Path.Exists(location.TextValue)
            ? location.TextValue
            : DefaultInstallLocation;
    }

    public Control CreateSettingPanel() => throw new NotImplementedException();


    private PluginInitContext? _context;

    /// <summary>
    /// Initialize the plugin with the given <see cref="PluginInitContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
    public void Init(PluginInitContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Required to implement the <see cref="IPlugin"/> interface.
    /// </summary>
    public List<Result> Query(Query query)
    {
        return [];
    }

    public List<Result> Query(Query query, bool isFullQuery) => Toolbox
        .LoadProjects()
        .Select(project => new Result
        {
            Title = project.Name,
            SubTitle = project.DisplayPath,
            Action = _ =>
            {
                try
                {
                    Process.Start(project.Ide.Executable, project.Path);
                    Log.Info($"Process started with {project.Ide.Executable} in {project.Path}", typeof(Main));
                    return true;
                }
                catch
                {
                    Log.Warn($"Process not started with {project.Ide.Executable} in {project.Path}", typeof(Main));
                    return false;
                }
            }
        })
        .ToList();
}