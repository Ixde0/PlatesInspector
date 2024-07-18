using System;
using System.Numerics;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;

namespace PlatesInspector.Windows;

public class MainWindow : Window, IDisposable
{
    private string treeImagePath;
    private PlatesInspectorPlugin piPlugin;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(PlatesInspectorPlugin plugin, string imagePath)
        : base("My Amazing Window##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        treeImagePath = imagePath;
        piPlugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text($"The random config bool is {piPlugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        if (ImGui.Button("Show Settings"))
        {
            piPlugin.ToggleConfigUI();
        }

        ImGui.Spacing();

        ImGui.Text("Cute tree :3");
        var treeImage = PlatesInspectorPlugin.TextureProvider.GetFromFile(treeImagePath).GetWrapOrDefault();
        if (treeImage != null)
        {
            ImGuiHelpers.ScaledIndent(55f);
            ImGui.Image(treeImage.ImGuiHandle, new Vector2(treeImage.Width, treeImage.Height));
            ImGuiHelpers.ScaledIndent(-55f);
        }
        else
        {
            ImGui.Text("Image not found.");
        }
    }
}
