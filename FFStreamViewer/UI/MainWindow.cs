using System;
using System.Diagnostics;
using System.Numerics;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Utility;
using ImGuiNET;
using OtterGui.Widgets;
using FFStreamViewer.UI.Tabs.MediaTab;

namespace FFStreamViewer.UI;
/// <summary> This class is used to handle the main window. </summary>
public class MainWindow : Window
{
    public enum TabType {
        None            = -1,   // No tab selected
        Media           = 0,    // Where you select your gags and safewords and lock types.
        WebAPITesting   = 1,    // Where you can test the WebAPI
    }
    private readonly    FFSV_Config      _config;
    private readonly    FFStreamViewerChangelog   _changelog;
    private readonly    ITab[]              _tabs;
    public readonly     MediaTab            Media;
    public readonly     WebAPITestingTab    WebAPITesting;
    public              TabType             SelectTab = TabType.None;

    /// <summary> Initializes a new instance of the <see cref="MainWindow"/> class.
    /// <list type="bullet">
    /// <item><c>pluginInt</c><param name="pluginInt"> - The IDalamudPluginInterface.</param></item>
    /// <item><c>config</c><param name="config"> - The FFStreamViewer configuration.</param></item>
    /// <item><c>general</c><param name="general"> - The general tab.</param></item>
    /// <item><c>changelog</c><param name="changelog"> - The changelog.</param></item>
    /// </list> </summary>
    public MainWindow(IDalamudPluginInterface pluginInt, FFSV_Config config, MediaTab media,
    WebAPITestingTab webAPITestingTab, FFStreamViewerChangelog changelog): base(GetLabel()) {
        // let the user know if their direct chat garlber is still enabled upon launch
        // Let's first make sure that we disable the plugin while inside of gpose.
        pluginInt.UiBuilder.DisableGposeUiHide = true;

        // Next let's set the size of the window
        SizeConstraints = new WindowSizeConstraints() {
            MinimumSize = new Vector2(400, 300),     // Minimum size of the window
            MaximumSize = ImGui.GetIO().DisplaySize, // Maximum size of the window
        };

        // set the private readonly's to the passed in data of the respective names
        Media = media;
        WebAPITesting = webAPITestingTab;
        
        // Below are the stuff besides the tabs that are passed through
        //_event     = @event;
        _config    = config;
        _changelog = changelog;
        // the tabs to be displayed
        _tabs = new ITab[]
        {
            media,
            webAPITestingTab,
        };
    }

    public override void Draw() {
        // get our cursors Y position and store it into YPOS
        var yPos = ImGui.GetCursorPosY();
        // set the cursor position to the top left of the window
        if (TabBar.Draw("##tabs", ImGuiTabBarFlags.None, ToLabel(SelectTab), out var currentTab, () => { }, _tabs)) {
            SelectTab           = TabType.None; // set the selected tab to none
            _config.SelectedTab = FromLabel(currentTab); // set the config selected tab to the current tab
            _config.Save(); // FIND OUT HOW TO USE SaveConfig(); ACROSS CLASSES LATER.
        }
        // We want to display the save & close, and the donation buttons on the topright, so lets draw those as well.
        ImGui.SetCursorPos(new Vector2(ImGui.GetWindowContentRegionMax().X - 9f * ImGui.GetFrameHeight(), yPos - ImGuiHelpers.GlobalScale));
        ImGui.PushStyleColor(ImGuiCol.Button, 0xFF000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA000000 | 0x005E5BFF);
        ImGui.Text(" ");
        ImGui.SameLine();
        if (ImGui.Button("Changelog")) {
            // force open the changelog here
            _changelog.Changelog.ForceOpen = true;
        }
        // In that same line...
        ImGui.SameLine();
        // And now have that button be for the Ko-Fi Link
        if (ImGui.Button("Toss Cordy a thanks!")) {
            ImGui.SetTooltip( "Only if you want to though!");
            Process.Start(new ProcessStartInfo {FileName = "https://ko-fi.com/cordeliamist", UseShellExecute = true});
        }
        // pop off the colors we pushed
        ImGui.PopStyleColor(3);
    }

    /// <summary> This function is used to draw the changelog window. </summary>
    private void DrawChangeLog(Changelog changelog) {
        // Draw the changelog
        changelog.ForceOpen = true;
    }

    /// <summary> 
    /// This function is used to draw the changelog window.
    /// <list type="bullet">
    /// <item><c>TabType</c><param name="type"> - the type of tab we want to go to.</param></item>
    /// </list> </summary> 
    private ReadOnlySpan<byte> ToLabel(TabType type)
        => type switch // we do this via a switch statement
        {
            TabType.Media         => Media.Label,
            TabType.WebAPITesting => WebAPITesting.Label,
            _                     => ReadOnlySpan<byte>.Empty, // This label confuses me a bit. I think it is just a blank label?
        };


    /// <summary>
    /// The function used to say what tab we are going from.
    /// <list type="bullet">
    /// <item><c>label</c><param name="label"> - the label of the tab we are going from.</param></item>
    /// </list> </summary> 
    private TabType FromLabel(ReadOnlySpan<byte> label) {
        // @formatter:off
        if (label == Media.Label)         return TabType.Media;
        if (label == WebAPITesting.Label) return TabType.WebAPITesting;
        // @formatter:on
        return TabType.None;
    }

    // basic string function to get the label of title for the window
    private static string GetLabel() => "FFStreamViewer###FFStreamViewerMainWindow";
}