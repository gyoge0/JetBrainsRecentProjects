namespace Community.PowerToys.Run.Plugin.JetBrainsRecentProjects;

using Wox.Plugin;

// ReSharper disable once UnusedType.Global
public class Main : IPlugin
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
    public static string PluginID => "E3318C75-337A-4A11-918A-90BB4F909502";

    public string Name => "JetBrains Recent Projects";

    public string Description => "Open recent projects from JetBrains IDEs";

    private PluginInitContext? _context;

    /// <summary>
    /// Initialize the plugin with the given <see cref="PluginInitContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
    public void Init(PluginInitContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public List<Result> Query(Query query)
    {
        return
        [
            new Result
            {
                QueryTextDisplay = query.Search,
                IcoPath = null,
                Title = "Test Title",
                SubTitle = "Test Subtitle",
                ToolTipData = new ToolTipData("Test", $"Test tooltip"),
                ContextData = null,
            },
        ];
    }
}