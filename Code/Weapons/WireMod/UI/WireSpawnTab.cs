[Title( "Wiremod" ), Order( 50 ), Icon( "⚡" )]
public class WireSpawnTab : BaseSpawnMenu
{
    static Dictionary<string, string> CategoryIcons = new()
    {
        { "Gates", "🔀" },
        { "Arithmetic", "🔢" },
        { "Memory", "💾" },
        { "Timer", "⏱️" },
        { "String", "📝" },
        { "IO Inputs", "🎮" },
        { "IO Outputs", "📢" },
        { "Entity", "🎯" },
        { "Display", "📺" },
        { "Sensors", "📡" },
        { "Array", "📊" },
        { "CPU", "🖥️" },
        { "Vehicle", "🚗" },
        { "Converter", "🔄" },
        { "Debug", "🐛" },
    };

    protected override void Rebuild()
    {
        var allTypes = Game.TypeLibrary.GetTypes<WireComponent>()
            .Where(t => !t.IsAbstract)
            .OrderBy(t => t.Group)
            .ThenBy(t => t.Title)
            .ToList();

        var grouped = allTypes.GroupBy(t => t.Group);
        foreach (var group in grouped)
        {
            var groupName = group.Key;
            var icon = CategoryIcons.GetValueOrDefault(groupName, "⚡");
            var items = group.Select(t => new WireEntityItem
            {
                Title = t.Title,
                Icon = t.Icon,
                ClassName = t.ClassName,
                Group = t.Group
            }).ToList();

            var capturedItems = items;
            AddOption(icon, groupName, () => new WireEntityGrid { Types = capturedItems });
        }
    }
}

public class WireEntityItem
{
    public string Title { get; set; }
    public string Icon { get; set; }
    public string ClassName { get; set; }
    public string Group { get; set; }
}
