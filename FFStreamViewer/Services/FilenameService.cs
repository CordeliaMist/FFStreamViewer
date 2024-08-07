using Dalamud.Plugin;
using OtterGui.Log; // Used for the IDalamudPluginInterface which provides access to various Dalamud features

namespace FFStreamViewer.Services;

/// <summary> Service for managing file names. </summary>
public class FilenameService
{
    public readonly string ConfigDirectory; // Directory for the configuration files
    public readonly string ConfigFile;      // Configuration file

    /// <summary>
    /// Initializes a new instance of the <see cref="FilenameService"/> class.
    /// <list type="bullet">
    /// <item><c>pi</c><param name="pi"> - The Dalamud plugin interface.</param></item>
    /// </list> </summary>
    public FilenameService(IDalamudPluginInterface pi) {
        // Set the configuration directory and file from the plugin interface
        ConfigDirectory = pi.ConfigDirectory.FullName;
        ConfigFile = pi.ConfigFile.FullName;
    }
}
