namespace FFStreamViewer.WebAPI.GagspeakConfiguration.Models;

/// <summary>
/// A basic authentication class to validate that the information from the client when they attempt to connect is correct.
/// </summary>
[Serializable]
public record RestraintSet
{
    /// <summary> The name of the pattern </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary> The description of the pattern </summary>
    public string Description { get; set; } = string.Empty;

    public bool Enabled { get; set; } = false;

    public bool Locked { get; set; } = false;

    public string EnabledBy { get; set; } = string.Empty;

    public string LockedBy { get; set; } = string.Empty;

    public DateTimeOffset LockedUntil { get; set; } = DateTimeOffset.MinValue;

    // add drawdata and associated mods here later after migration
}
