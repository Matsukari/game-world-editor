using ImGuiNET;

namespace Raven.Widget
{ 
  public class NameModal
  {
    bool _isNameModal;
    Action<string> _nameCallback;
    public void Open(Action<string> callback)
    {
      _nameCallback = callback;
      _isNameModal = true;
    }
    public void Draw()
    {
      if (_isNameModal)
      {
        ImGui.OpenPopup("name-action-modal");
        _isNameModal = false;
      }
      var open = true;
      if (ImGui.BeginPopupModal("name-action-modal", ref open, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize))
      {
        var input = "";
        ImGui.SetKeyboardFocusHere();
        if (ImGui.InputText("Name", ref input, 50, ImGuiInputTextFlags.EnterReturnsTrue))
        {
          _nameCallback.Invoke(input);
          ImGui.CloseCurrentPopup();
          input = "";
        }
        ImGui.EndPopup();
      }
    } 
  }
}
