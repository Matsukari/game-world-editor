using ImGuiNET;
using Num = System.Numerics;

namespace Raven 
{
  public enum ConfirmState { Confirmed, Refused, Cancel }
}
namespace Raven.Widget
{ 
  public class ConfirmModal
  {
    bool _isConfirmModal;
    Action<ConfirmState> _confimCallback;
    string _title;
    string _dialog;
    public void Open(Action<ConfirmState> callback, string title, string dialog)
    {
      _confimCallback = callback;
      _isConfirmModal = true;
      _title = title;
      _dialog = dialog;
    }
    public void Draw()
    {
      if (_isConfirmModal)
      {
        ImGui.OpenPopup("confirm-action-modal");
        _isConfirmModal = false;
      }
      var open = true;
      if (ImGui.BeginPopupModal("confirm-action-modal", ref open, ImGuiWindowFlags.NoDecoration))
      {
        ImGui.SetWindowSize(new Num.Vector2(260, 200));

        ImGuiUtils.TextMiddleX(_title);

        ImGui.BeginChild(1, new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y-30));
        ImGuiUtils.TextMiddle(_dialog);
        ImGui.EndChild();

        var size = new Num.Vector2(ImGui.GetContentRegionAvail().X * 0.33f, 20); 
        if (ImGui.Button("Ok", size))
        {
          ImGui.CloseCurrentPopup();
          _confimCallback.Invoke(ConfirmState.Confirmed);
        }
        ImGui.SameLine();
        if (ImGui.Button("No", size))
        {
          ImGui.CloseCurrentPopup();
          _confimCallback.Invoke(ConfirmState.Refused);
        }
        ImGui.SameLine();
        if (ImGui.Button("Cancel", size))
        {
          ImGui.CloseCurrentPopup();
          _confimCallback.Invoke(ConfirmState.Cancel);
        }
        if (ImGui.IsKeyPressed(ImGuiKey.Escape)) 
        {
          ImGui.CloseCurrentPopup();
        }
        ImGui.EndPopup();
      }
    } 
  }
}
