﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Memory;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace PlatesInspector.Windows;

public unsafe class MainWindow : Window, IDisposable
{

    // private static AddonPartyList* AddonPartyList => (AddonPartyList*)Service.GameGui.GetAddonByName("_PartyList");

    private string treeImagePath;
    private PlatesInspectorPlugin piPlugin;

    private HashSet<AdvPlateData> playersData = [];
    private HashSet<AdvPlateData> history = [];
    private HashSet<AdvPlateData> currentlyShowing = [];

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(PlatesInspectorPlugin plugin, string imagePath)
        : base("PlatesInspector##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
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

    public override void OnOpen()
    {
        ReloadPlayers();
    }


    public struct AdvPlateData
    {
        public string playerName;
        public ulong contentId;
        public AdvPlateData(string playerName, ulong contentId)
        {
            this.playerName = playerName;
            this.contentId = contentId;
        }
    }

    public override void Draw()
    {
        // ImGui.Text($"The random config bool is {piPlugin.Configuration.SomePropertyToBeSavedAndWithADefault}")

        // if (ImGui.Button("Log party & alliance"))
        // {
        //     //piPlugin.ToggleConfigUI();
        // }

        if (ImGui.Button("Refresh"))
        {
            ReloadPlayers();
        }

        if (ImGui.Button("History"))
        {
            Service.Log.Info("Loading history. Players count: " + history.Count);
            this.currentlyShowing = history;
        }

        foreach (var player in currentlyShowing)
        {

            if (ImGui.Button(player.playerName))
            {
                ShowAdvPlate(player.contentId);
            }
        }

        ImGui.Spacing();

        // ImGui.Text(GetSomeInGameText());
        // var treeImage = PlatesInspectorPlugin.TextureProvider.GetFromFile(treeImagePath).GetWrapOrDefault();
        // if (treeImage != null)
        // {
        //     ImGuiHelpers.ScaledIndent(55f);
        //     ImGui.Image(treeImage.ImGuiHandle, new Vector2(treeImage.Width, treeImage.Height));
        //     ImGuiHelpers.ScaledIndent(-55f);
        // }
        // else
        // {
        //     ImGui.Text("Image not found.");
        // }
    }

    private unsafe string GetSomeInGameText()
    {
        var player = PlayerState.Instance();
        var playerName = player->CharacterNameString;
        var playerLvl = player->CurrentLevel;

        return "Hello, im " + playerName + ", my lvl is: " + playerLvl;
    }

    private unsafe void ShowAdvPlate(ulong contentId)
    {
        // var player = PlayerState.Instance();
        AgentCharaCard.Instance()->OpenCharaCard(contentId);
    }

    private void ReloadPlayers()
    {
        Service.Log.Info("Reloading players");

        var platesData = new HashSet<AdvPlateData>();

        var alliance = GroupManager.Instance()->GetGroup()->AllianceMembers.ToArray();
        foreach (var index in Enumerable.Range(0, alliance.Length))
        {
            var alliancePlayer = alliance[index];
            if (alliancePlayer.ContentId > 0)
            {
                //Service.Log.Info("Alliance member " + index + " -> name: " + alliancePlayer.NameString + ", contentId:" + alliancePlayer.ContentId);
                platesData.Add(new AdvPlateData(alliancePlayer.NameString, alliancePlayer.ContentId));
            }
        }

        var party = GroupManager.Instance()->GetGroup()->PartyMembers.ToArray();
        foreach (var index in Enumerable.Range(0, party.Length))
        {
            var partyMember = party[index];
            if (partyMember.ContentId > 0)
            {
                //Service.Log.Info("Party member " + index + " -> name: " + partyMember.NameString + ", contentId:" + partyMember.ContentId);
                platesData.Add(new AdvPlateData(partyMember.NameString, partyMember.ContentId));
            }
        }

        this.playersData = platesData;
        this.history.UnionWith(platesData);
        this.currentlyShowing = this.playersData;

        Service.Log.Info("Found " + platesData.Count + " players");
    }

    // TODO: 
    //  - load enemies in frontlines

}
