using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using FFStreamViewer.WebAPI.PlayerData.Pairs;
using FFStreamViewer.WebAPI.Services.Mediator;
using FFStreamViewer.WebAPI;
using System.Numerics;
using Dalamud.Interface.Utility;
using OtterGui;

namespace FFStreamViewer.WebAPI.UI.Components.Popup;

internal class VerificationPopupHandler : IStickyUiHandler
{
    private readonly ApiController _apiController;
    private readonly UiSharedService _uiSharedService;
    private string _verificationCode = string.Empty;

    public VerificationPopupHandler(ApiController apiController, UiSharedService uiSharedService)
    {
        _apiController = apiController;
        _uiSharedService = uiSharedService;
    }

    public Vector2 PopupSize => new(600, 160);

    public bool ShowClose => false;

    public void DrawContent()
    {
        var width = ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;
        // push the title for the popup.
        using (_uiSharedService.UidFont.Push())
        {
            var headerTextSize = ImGui.CalcTextSize("Verification Code for " + _uiSharedService.ApiController.DisplayName);
            ImGui.SetCursorPosX((ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X) / 2 - (headerTextSize.X / 2));
            UiSharedService.TextWrapped("Verification Code for " + _uiSharedService.ApiController.DisplayName);

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGuiHelpers.GlobalScale * 5);
            ImGui.Separator();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGuiHelpers.GlobalScale * 5);

            // canter the text input so that its width is the width of the _verificationCode text + 10px, centerd in the available context.
            ImGui.SetCursorPosX(width / 2 - ((ImGui.CalcTextSize(_verificationCode).X + ImGuiHelpers.GlobalScale * 12) / 2));

            ImGui.TextColored(ImGuiColors.ParsedPink, _verificationCode);
            UiSharedService.CopyableDisplayText(_verificationCode, "Click to copy verification code to clipboard");
        }
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGuiHelpers.GlobalScale * 10);
        ImGui.Separator();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGuiHelpers.GlobalScale * 10);
        ImGui.AlignTextToFramePadding();
        ImGui.SetCursorPosX(ImGui.GetWindowContentRegionMax().X / 2 - ImGui.CalcTextSize("Close Popup window &&& Be sure verification is successful before closing.").X / 2);
        using (ImRaii.Disabled(string.IsNullOrEmpty(_verificationCode)))
        {
            if (_uiSharedService.IconTextButton(FontAwesomeIcon.ExclamationTriangle, "Close Popup Window"))
            {
                ImGui.CloseCurrentPopup();
            }
        }
        ImGui.SameLine();
        UiSharedService.ColorTextWrapped($"Be sure verification is successful before closing.", ImGuiColors.DalamudYellow);

    }

    public void Open(VerificationPopupMessage msg)
    {
        // set the verification code after we open.
        _verificationCode = msg.VerificationCode.VerificationCode;
    }
}
