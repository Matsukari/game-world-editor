using ImGuiNET;
using Num = System.Numerics;

namespace Raven.Widget
{ 
  public abstract class OkCancelPopup 
  {
    bool _isokCancel;
    Action _okCancelCallback;
    public bool IsOpen { get => ImGui.IsPopupOpen("okCancel-action-modal"); }
    public void Open(Action callback, bool modal=true)
    {
      _okCancelCallback = callback;
      _isokCancel = true;
    }
    Num.Vector2 _winSize = new Num.Vector2(240, 74); 

    protected abstract void OnDraw();
    public void Draw()
    {
      if (_isokCancel)
      {
        ImGui.OpenPopup("okCancel-action-modal");
        _isokCancel = false;
      }
      if (ImGui.BeginPopup("okCancel-action-modal", ImGuiWindowFlags.NoDecoration))
      { 
        ImGui.SetWindowSize(_winSize);

        OnDraw();

        var size = new Num.Vector2(_winSize.X * 0.5f, 20);
        if (ImGui.Button("Ok", size))
        {
          ImGui.CloseCurrentPopup();
          _okCancelCallback.Invoke();
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
