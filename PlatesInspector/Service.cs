using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace PlatesInspector;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class Service {
        [PluginService] public static IGameGui GameGui { get; set; }
        [PluginService] public static IPluginLog Log { get; set; }

}