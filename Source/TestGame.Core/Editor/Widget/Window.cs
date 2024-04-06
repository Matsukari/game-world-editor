using ImGuiNET;
using Nez;

namespace Raven.Widget
{
  public class Window : IImGuiRenderable
  {
    public string Name = "";
    public ImGuiWindowFlags Flags = ImGuiWindowFlags.None;
    public RectangleF Bounds = new RectangleF();
    public bool IsOpen 
    { 
      get => _isOpen; 
      set 
      { 
        if (value == _isOpen) return;
        _isOpen = value;
        if (_isOpen && OnOpen != null) OnOpen();
        else if (OnClose != null) OnClose();
      }
    }
    public bool NoClose = true;

    protected bool _isOpen = true;

    public event Action OnClose;
    public event Action OnOpen;

    public Window() => Name = GetType().Name
      ;
    public virtual void OnRender(ImGuiWinManager imgui) {}

    public virtual void OnHovered() {}

    public virtual void Render(ImGuiWinManager imgui)
    {
      if (!IsOpen) return;

      if (NoClose) 
        ImGui.Begin(Name, Flags);
      else 
        ImGui.Begin(Name, ref _isOpen, Flags);

      if (ImGui.IsWindowHovered()) 
      {
        OnHovered();
        ImGui.SetWindowFocus();
      }
      Bounds.Location = ImGui.GetWindowPos();
      Bounds.Size = ImGui.GetWindowSize();
      OnRender(imgui);
      ImGui.End();  
    }
  }
}
