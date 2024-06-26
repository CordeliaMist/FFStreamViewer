using System.Numerics;

namespace FFStreamViewer.WebAPI.UI.Components.Popup;

/// <summary> A interface for handling the popups in the UI. </summary>
public interface IStickyUiHandler
{
    Vector2 PopupSize { get; }
    bool ShowClose { get; }

    void DrawContent();
}
