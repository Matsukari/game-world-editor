using ImGuiNET;
using Num = System.Numerics;

namespace Raven.Widget
{ 
  public class PopupDelegate<T> : IImGuiRenderable
  {
    bool _isOpen;
    public T Capture;
    public string Name;
    Action<ImGuiWinManager> _renderDelegate;
    public bool IsOpen { get => ImGui.IsPopupOpen(Name); }
    public PopupDelegate(string name) => Name = name;
    public void Open(Action<ImGuiWinManager> renderDelegate, T capture)
    {
      _isOpen = true;
      Capture = capture;
      _renderDelegate = renderDelegate;
    }

    public void Render(ImGuiWinManager imgui)
    {
      if (_isOpen)
      {
        ImGui.OpenPopup(Name);
        _isOpen = false;
      }
      if (ImGui.BeginPopup(Name, ImGuiWindowFlags.NoDecoration))
      {
        _renderDelegate.Invoke(imgui);
        ImGui.EndPopup();
      }
    } 
  }
  public class PopupDelegate : IImGuiRenderable
  {
    bool _isOpen;
    public string Name;
    Action<ImGuiWinManager> _renderDelegate;
    public bool IsOpen { get => ImGui.IsPopupOpen(Name); }
    public PopupDelegate(string name) => Name = name;
    public void Open(Action<ImGuiWinManager> renderDelegate)
    {
      _isOpen = true;
      _renderDelegate = renderDelegate;
    }

    public void Render(ImGuiWinManager imgui)
    {
      if (_isOpen)
      {
        ImGui.OpenPopup(Name);
        _isOpen = false;
      }
      if (ImGui.BeginPopup(Name, ImGuiWindowFlags.NoDecoration))
      {
        _renderDelegate.Invoke(imgui);
        ImGui.EndPopup();
      }
    } 
  }
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
