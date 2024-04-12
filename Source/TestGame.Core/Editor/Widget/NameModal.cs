using ImGuiNET;
using Num = System.Numerics;

namespace Raven.Widget
{ 
  public class NameModal
  {
    bool _isNameModal;
    Action<string> _nameCallback;
    public void Open(Action<string> callback, bool modal=true)
    {
      _nameCallback = callback;
      _isNameModal = true;
    }
    string _input = "";
    public void Draw()
    {
      if (_isNameModal)
      {
        ImGui.OpenPopup("name-action-modal");
        _isNameModal = false;
      }
      var open = true;
      if (ImGui.BeginPopupModal("name-action-modal", ref open, ImGuiWindowFlags.NoDecoration))
      {
        ImGui.SetWindowSize(new Num.Vector2(240, 74));

        ImGuiUtils.TextMiddleX("Enter a name");
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.InputText("##2", ref _input, 50, ImGuiInputTextFlags.EnterReturnsTrue))
        {
          _nameCallback.Invoke(_input);
          ImGui.CloseCurrentPopup();
          _input = "";
          _nameCallback = null;
        }
        var size = new Num.Vector2(ImGui.GetContentRegionAvail().X * 0.5f, 20);
        if (ImGui.Button("Ok", size))
        {
          ImGui.CloseCurrentPopup();
          _nameCallback.Invoke(_input);
        }
        ImGui.SameLine();
        if (ImGui.Button("Cancel", size))
        {
          ImGui.CloseCurrentPopup();
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
